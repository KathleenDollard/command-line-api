// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace System.CommandLine.Parsing
{
    internal static class StringExtensions
    {
        /*
        internal static bool ContainsCaseInsensitive(
            this string source,
            string value) =>
            source.IndexOfCaseInsensitive(value) >= 0;

        internal static int IndexOfCaseInsensitive(
            this string source,
            string value) =>
            CultureInfo.InvariantCulture
                       .CompareInfo
                       .IndexOf(source,
                                value,
                                CompareOptions.OrdinalIgnoreCase);
        */
    }

    /* See Tokenizer.cs
    internal static class CliTokenizer
    {
        internal static (string? Prefix, string Alias) SplitPrefix(string rawAlias)
        {
            // TODO: I believe this code would be faster and easier to understand with collection patterns
            if (rawAlias[0] == '/')
            {
                return ("/", rawAlias.Substring(1));
            }
            else if (rawAlias[0] == '-')
            {
                if (rawAlias.Length > 1 && rawAlias[1] == '-')
                {
                    return (doubleDash, rawAlias.Substring(2));
                }

                return ("-", rawAlias.Substring(1));
            }

            return (null, rawAlias);
        }

        // TODO: What does the following comment do, and do we need it
        // this method is not returning a Value Tuple or a dedicated type to avoid JITting
        /* This method had some work done, and then was returned to basic start code. Use main branch for reference as I may have messed up.
        internal static void Tokenize(
            IReadOnlyList<string> args,
            CliCommand rootCommand,
            bool inferRootCommand,
            bool enablePosixBundling,
            int skipArgs,
            out List<CliToken> tokens,
            out List<string>? errors)
        {
            const int FirstArgIsNotRootCommand = -1;

            List<string>? errorList = null;

            var currentCommand = rootCommand;
            var foundDoubleDash = false;
            var foundEndOfDirectives = false;

            var tokenList = new List<CliToken>(args.Count);

            var knownTokens = GetValidTokens(rootCommand);

            int i = FirstArgumentIsRootCommand(args, rootCommand, inferRootCommand)
                ? 0
                : FirstArgIsNotRootCommand;


            var arg = rootCommand.Name;
            if (knownTokens.TryGetValue(arg, out var rootToken))
            {
                CliCommand cmd = (CliCommand)rootToken.Symbol!;
                currentCommand = cmd;
                tokenList.Add(Command(arg, cmd, firstArgIsRootCommand ? 0 : -1));
            }
            var startPosition = skipArgs + (firstArgIsRootCommand ? 1 : 0);

            for (var i = startPosition; i < args.Count; i++)
            {
                 arg = i == FirstArgIsNotRootCommand
                    ? rootCommand.Name
                    : args[i];
                arg = args[i];

                if (foundDoubleDash)
                {
                    tokenList.Add(CommandArgument(arg, currentCommand!, i));

                    continue;
                }

                if (!foundDoubleDash &&
                    arg == "--")
                {
                    tokenList.Add(DoubleDash( i));
                    foundDoubleDash = true;
                    continue;
                }

                if (!foundEndOfDirectives)
                {
                    if (arg.Length > 2 &&
                        arg[0] == '[' &&
                        arg[1] != ']' &&
                        arg[1] != ':' &&
                        arg[arg.Length - 1] == ']')
                    {
                        int colonIndex = arg.AsSpan().IndexOf(':');
                        string directiveName = colonIndex > 0
                            ? arg.Substring(1, colonIndex - 1) // [name:value]
                            : arg.Substring(1, arg.Length - 2); // [name] is a legal directive

                        CliDirective? directive;
                        if (knownTokens.TryGetValue($"[{directiveName}]", out var directiveToken))
                        {
                            directive = (CliDirective)directiveToken.Symbol!;
                        }
                        else
                        {
                            directive = null;
                        }

                        tokenList.Add(Directive(arg, directive));
                        continue;
                    }

                    if (!configuration.RootCommand.EqualsNameOrAlias(arg))
                    {
                        foundEndOfDirectives = true;
                    }
                }

                if (configuration.ResponseFileTokenReplacer is { } replacer &&
                    arg.GetReplaceableTokenValue() is { } value)
                {
                    if (replacer(
                            value,
                            out var newTokens,
                            out var error))
                    {
                        if (newTokens is not null && newTokens.Count > 0)
                        {
                            List<string> listWithReplacedTokens = args.ToList();
                            listWithReplacedTokens.InsertRange(i + 1, newTokens);
                            args = listWithReplacedTokens;
                        }
                        continue;
                    }
                    else if (!string.IsNullOrWhiteSpace(error))
                    {
                        (errorList ??= new()).Add(error!);
                        continue;
                    }
                }

                if (knownTokens.TryGetValue(arg, out var token))
                {
                    if (PreviousTokenIsAnOptionExpectingAnArgument(out var option))
                    {
                        tokenList.Add(OptionArgument(arg, option!, i));
                    }
                    else
                    {
                        switch (token.Type)
                        {
                            case CliTokenType.Option:
                                tokenList.Add(Option(arg, (CliOption)token.Symbol!, i));
                                break;

                            case CliTokenType.Command:
                                CliCommand cmd = (CliCommand)token.Symbol!;
                                if (cmd != currentCommand)
                                {
                                    if (cmd != rootCommand)
                                    {
                                        knownTokens = GetValidTokens(cmd); // config contains Directives, they are allowed only for RootCommand
                                    }
                                    currentCommand = cmd;
                                    tokenList.Add(Command(arg, cmd, i));
                                }
                                else
                                {
                                    tokenList.Add(Argument(arg, i));
                                }

                                break;
                        }
                    }
                }
                else if (TrySplitIntoSubtokens(arg, out var first, out var rest) &&
                         knownTokens.TryGetValue(first, out var subtoken) &&
                         subtoken.Type == CliTokenType.Option)
                {
                    tokenList.Add(Option(first, (CliOption)subtoken.Symbol!, i));

                    if (rest is not null)
                    {
                        tokenList.Add(Argument(rest, i));
                    }
                }
                else if (!enablePosixBundling ||
                         !CanBeUnbundled(arg) ||
                         !TryUnbundle(arg.AsSpan(1), i))
                {
                    tokenList.Add(Argument(arg, i));
                }
            }

            static CliToken Argument(string value, int i) => new(value, CliTokenType.Argument, default, i);
             
            static CliToken CommandArgument(string value, CliCommand command, int i) => new(value, CliTokenType.Argument, command, i);
             
            static CliToken OptionArgument(string value, CliOption option, int i) => new(value, CliTokenType.Argument, option, i);
             
            static CliToken Command(string value, CliCommand cmd, int i) => new(value, CliTokenType.Command, cmd, i);
             
            static CliToken Option(string value, CliOption option, int i) => new(value, CliTokenType.Option, option, i);
             
            static CliToken DoubleDash(int i) => new("--", CliTokenType.DoubleDash, default, i);

            // TODO: Directives
            //             CliToken Directive(string value, CliDirective? directive) => new(value, CliTokenType.Directive, directive, i);

            tokens = tokenList;
            errors = errorList;

            bool CanBeUnbundled(string arg)
                => arg.Length > 2
                    && arg[0] == '-'
                    && arg[1] != '-'// don't check for "--" prefixed args
                    && arg[2] != ':' && arg[2] != '=' // handled by TrySplitIntoSubtokens
                    && !PreviousTokenIsAnOptionExpectingAnArgument(out _);

            bool TryUnbundle(ReadOnlySpan<char> alias, int argumentIndex)
            {
                int tokensBefore = tokenList.Count;

                string candidate = new('-', 2); // mutable string used to avoid allocations
                unsafe
                {
                    fixed (char* pCandidate = candidate)
                    {
                        for (int i = 0; i < alias.Length; i++)
                        {
                            if (alias[i] == ':' || alias[i] == '=')
                            {
                                tokenList.Add(new CliToken(alias.Slice(i + 1).ToString(), CliTokenType.Argument, default, argumentIndex));
                                return true;
                            }

                            pCandidate[1] = alias[i];
                            if (!knownTokens.TryGetValue(candidate, out CliToken? found))
                            {
                                if (tokensBefore != tokenList.Count && tokenList[tokenList.Count - 1].Type == CliTokenType.Option)
                                {
                                    // Invalid_char_in_bundle_causes_rest_to_be_interpreted_as_value
                                    tokenList.Add(new CliToken(alias.Slice(i).ToString(), CliTokenType.Argument, default, argumentIndex));
                                    return true;
                                }

                                return false;
                            }

                            tokenList.Add(new CliToken(found.Value, found.Type, found.Symbol, argumentIndex));
                            if (i != alias.Length - 1 && ((CliOption)found.Symbol!).Greedy)
                            {
                                int index = i + 1;
                                if (alias[index] == ':' || alias[index] == '=')
                                {
                                    index++; // Last_bundled_option_can_accept_argument_with_colon_separator
                                }
                                tokenList.Add(new CliToken(alias.Slice(index).ToString(), CliTokenType.Argument, default, argumentIndex));
                                return true;
                            }
                        }
                    }
                }

                return true;
            }

            bool PreviousTokenIsAnOptionExpectingAnArgument(out CliOption? option)
            {
                if (tokenList.Count > 1)
                {
                    var token = tokenList[tokenList.Count - 1];

                    if (token.Type == CliTokenType.Option)
                    {
                        if (token.Symbol is CliOption { Greedy: true } opt)
                        {
                            option = opt;
                            return true;
                        }
                    }
                }

                option = null;
                return false;
            }
        }
    }
        */

}