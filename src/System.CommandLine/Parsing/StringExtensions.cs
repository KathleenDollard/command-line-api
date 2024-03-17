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

    internal static class Tokenizer
    {
        private const string doubleDash = "--";

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

        // TODO: When would we ever not infer the rootcommand? This might have been to solve a bug where the first argument could not be the name of the root command.
        internal static void Tokenize(
            IReadOnlyList<string> args,
            CliCommand rootCommand,
            CliConfiguration configuration,
            bool inferRootCommand,
            bool enablePosixBundling,
            out List<CliToken> tokens,
            out List<string>? errors)
        {
            tokens = new List<CliToken>(args.Count);

            // Handle exe not being in args here?
            var exeNamePosition = FirstArgIsRootCommand(args, rootCommand, inferRootCommand)
                                    ? 0
                                    : -1;
            var rootLocation = Location.CreateUser(-1, rootCommand.Name.Length);
            if (exeNamePosition == -1)
            {
                tokens.Add(Command(rootCommand.Name, rootCommand, rootLocation));
            }

            var knownTokens = GetValidTokens(rootCommand);
            var newErrors = MapTokens(args,
                      rootLocation,
                      rootCommand,
                      null,
                      knownTokens,
                      configuration,
                      enablePosixBundling,
                      false,
                      tokens);

            errors = newErrors;

            static List<string>? MapTokens(IReadOnlyList<string> args,
                                           Location location,
                                           CliCommand currentCommand,
                                           CliOption? currentOption,
                                           Dictionary<string, CliToken> knownTokens,
                                           CliConfiguration configuration,
                                           bool enablePosixBundling,
                                           bool foundDoubleDash,
                                           List<CliToken> tokens)
            {
                List<string>? errors = null;
                var previousOptionWasClosed = false;

                for (var i = 0; i < args.Count; i++)
                {
                    var arg = args[i];

                    if (foundDoubleDash)
                    {
                        // everything after the double dash is added as an argument
                        tokens.Add(CommandArgument(arg, currentCommand!, Location.FromOuterLocation( i, arg.Length, location)));
                        continue;
                    }

                    if (arg == doubleDash)
                    {
                        tokens.Add(DoubleDash(i, Location.FromOuterLocation( i, doubleDash.Length,location)));
                        foundDoubleDash = true;
                        continue;
                    }

                    // TODO: Figure out a place to put this test, or at least the prefix, somewhere not hard-coded
                    if (configuration.ResponseFileTokenReplacer is not null &&
                        arg.StartsWith("@"))
                    {
                        var responseName = arg.Substring(1);
                        var (insertArgs, insertErrors) = configuration.ResponseFileTokenReplacer(responseName);
                        // TODO: Handle errors
                        if (insertArgs is not null && insertArgs.Any())
                        {
                            var innerLocation = Location.CreateResponse(responseName, i, arg.Length, location); 
                            var newErrors = MapTokens(insertArgs, innerLocation, currentCommand,
                                currentOption, knownTokens, configuration, enablePosixBundling, foundDoubleDash, tokens);
                        }
                        continue;
                    }

                    if (knownTokens.TryGetValue(arg, out var token))
                    {
                        // This test and block is to handle the case `-x -x` where -x takes a string arg and "-x" is the value. Normal 
                        // option argument parsing is handled as all other arguments, because it is not a known token.
                        if (PreviousTokenIsAnOptionExpectingAnArgument(out var option, tokens, previousOptionWasClosed))
                        {
                            tokens.Add(OptionArgument(arg, option!, Location.FromOuterLocation( i, arg.Length, location)));
                            continue;
                        }
                        else
                        {
                            currentCommand = AddKnownToken(currentCommand, tokens, ref knownTokens, arg,
                                Location.FromOuterLocation( i, arg.Length, location), token);
                            previousOptionWasClosed = false;
                        }
                    }
                    else
                    {
                        if (TrySplitIntoSubtokens(arg, out var first, out var rest) &&
                             knownTokens.TryGetValue(first, out var subToken) &&
                             subToken.Type == CliTokenType.Option)
                        {
                            CliOption option = (CliOption)subToken.Symbol!;
                            tokens.Add(Option(first, option, Location.FromOuterLocation(i, first.Length, location)));

                            if (rest is not null)
                            {
                                rest = option.ClosedBy is not null
                                    ? rest.Substring(0, rest.Length - option.ClosedBy.Length)
                                    : rest;
                                tokens.Add(Argument(rest,  Location.FromOuterLocation( i, rest.Length, location, first.Length + 1)));
                            }
                        }
                        else if (!enablePosixBundling ||
                                 !CanBeUnbundled(arg, tokens) ||
                                 !TryUnbundle(arg.AsSpan(1), Location.FromOuterLocation(i, arg.Length, location), knownTokens, tokens))
                        {
                            tokens.Add(Argument(arg, Location.FromOuterLocation(i, arg.Length, location)));
                        }
                    }
                }

                return errors;
            }

            static bool CanBeUnbundled(string arg, List<CliToken> tokenList)
                => arg.Length > 2
                    && arg[0] == '-'
                    && arg[1] != '-'// don't check for "--" prefixed args
                    && arg[2] != ':' && arg[2] != '=' // handled by TrySplitIntoSubtokens
                    && !PreviousTokenIsAnOptionExpectingAnArgument(out _, tokenList, false);

            static bool TryUnbundle(ReadOnlySpan<char> alias,
                                    Location outerLocation,
                                    Dictionary<string, CliToken> knownTokens,
                                    List<CliToken> tokenList)
            {
                int tokensBefore = tokenList.Count;
                // TODO: Determine if these pointers are helping us enough for complexity
                string candidate = new('-', 2); // mutable string used to avoid allocations
                unsafe
                {
                    fixed (char* pCandidate = candidate)
                    {
                        for (int i = 0; i < alias.Length; i++)
                        {
                            if (alias[i] == ':' || alias[i] == '=')
                            {
                                string value = alias.Slice(i + 1).ToString();
                                tokenList.Add(Argument(value,
                                    Location.FromOuterLocation( outerLocation.Start, value.Length, outerLocation, i + 1)));
                                return true;
                            }

                            pCandidate[1] = alias[i];
                            if (!knownTokens.TryGetValue(candidate, out CliToken? found))
                            {
                                if (tokensBefore != tokenList.Count && tokenList[tokenList.Count - 1].Type == CliTokenType.Option)
                                {
                                    // Invalid_char_in_bundle_causes_rest_to_be_interpreted_as_value
                                    string value = alias.Slice(i).ToString();
                                    tokenList.Add(Argument(value,
                                        Location.FromOuterLocation(outerLocation.Start, value.Length, outerLocation, i)));
                                    return true;
                                }

                                return false;
                            }

                            tokenList.Add(CliToken.CreateFromOtherToken(found, found.Value,
                                Location.FromOuterLocation(outerLocation.Start, found.Value.Length, outerLocation, i + 1)));

                            if (i != alias.Length - 1 && ((CliOption)found.Symbol!).Greedy)
                            {
                                int index = i + 1;
                                if (alias[index] == ':' || alias[index] == '=')
                                {
                                    index++; // Last_bundled_option_can_accept_argument_with_colon_separator
                                }

                                string value = alias.Slice(index).ToString();
                                tokenList.Add(Argument(value, Location.FromOuterLocation(outerLocation.Start, value.Length, outerLocation, index)));
                                return true;
                            }
                        }
                    }
                }

                return true;
            }

            static bool PreviousTokenIsAnOptionExpectingAnArgument(out CliOption? option, List<CliToken> tokenList, bool previousOptionWasClosed)
            {
                if (tokenList.Count > 1)
                {
                    var token = tokenList[tokenList.Count - 1];

                    if (token.Type == CliTokenType.Option)// && !previousOptionWasClosed)
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

            static CliCommand AddKnownToken(CliCommand currentCommand,
                                            List<CliToken> tokenList,
                                            ref Dictionary<string, CliToken> knownTokens,
                                            string arg,
                                            Location location,
                                            CliToken token)
            {
                //var location = Location.FromOuterLocation(outerLocation, argPosition, arg.Length);
                switch (token.Type)
                {
                    case CliTokenType.Option:
                        var option = (CliOption)token.Symbol!;
                        tokenList.Add(Option(arg, option, location));
                        break;

                    case CliTokenType.Command:
                        // All arguments are initially classified as commands because they might be
                        CliCommand cmd = (CliCommand)token.Symbol!;
                        if (cmd != currentCommand)
                        {
                            currentCommand = cmd;
                            // TODO: In the following determine how the cmd could be RootCommand AND the cmd not equal currentCmd. This looks like it would always be true.. If it is a massive side case, is it important not to double the ValidTokens call?
                            if (true)  // cmd != rootCommand)
                            {
                                knownTokens = GetValidTokens(cmd); // config contains Directives, they are allowed only for RootCommand
                            }
                            tokenList.Add(Command(arg, cmd, location));
                        }
                        else
                        {
                            tokenList.Add(Argument(arg, location));
                        }

                        break;
                }
                return currentCommand;
            }
        }

        private static bool FirstArgIsRootCommand(IReadOnlyList<string> args, CliCommand rootCommand, bool inferRootCommand)
        {
            if (args.Count > 0)
            {
                if (inferRootCommand && args[0] == CliExecutable.ExecutablePath)
                {
                    return true;
                }

                try
                {
                    var potentialRootCommand = Path.GetFileName(args[0]);

                    if (rootCommand.EqualsNameOrAlias(potentialRootCommand))
                    {
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // possible exception for illegal characters in path on .NET Framework
                }
            }

            return false;
        }

        private static string? GetReplaceableTokenValue(string arg) =>
            arg.Length > 1 && arg[0] == '@'
                ? arg.Substring(1)
                : null;

        // TODO: Naming rules - sub-tokens has a dash and thus should be SubToken
        private static bool TrySplitIntoSubtokens(
            string arg,
            out string first,
            out string? rest)
        {
            var i = arg.AsSpan().IndexOfAny(':', '=');

            if (i >= 0)
            {
                first = arg.Substring(0, i);
                rest = arg.Substring(i + 1);
                if (rest.Length == 0)
                {
                    rest = null;
                }

                return true;
            }

            first = arg;
            rest = null;
            return false;
        }

        // TODO: rename to TryTokenizeResponseFile or TryTokenizeAdditionalResponse
        internal static bool TryReadResponseFile(
            string filePath,
            out IReadOnlyList<string>? newTokens,
            out string? error)
        {
            try
            {
                newTokens = ExpandResponseFile(filePath).ToArray();
                error = null;
                return true;
            }
            catch (FileNotFoundException)
            {
                error = LocalizationResources.ResponseFileNotFound(filePath);
            }
            catch (IOException e)
            {
                error = LocalizationResources.ErrorReadingResponseFile(filePath, e);
            }

            newTokens = null;
            return false;

            static IEnumerable<string> ExpandResponseFile(string filePath)
            {
                var lines = File.ReadAllLines(filePath);

                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];

                    foreach (var p in SplitLine(line))
                    {
                        if (GetReplaceableTokenValue(p) is { } path)
                        {
                            foreach (var q in ExpandResponseFile(path))
                            {
                                yield return q;
                            }
                        }
                        else
                        {
                            yield return p;
                        }
                    }
                }
            }

            static IEnumerable<string> SplitLine(string line)
            {
                var arg = line.Trim();

                if (arg.Length == 0 || arg[0] == '#')
                {
                    yield break;
                }

                foreach (var word in CliParser.SplitCommandLine(arg))
                {
                    yield return word;
                }
            }
        }

        private static Dictionary<string, CliToken> GetValidTokens(CliCommand command)
        {
            Dictionary<string, CliToken> tokens = new(StringComparer.Ordinal);

            AddCommandTokens(tokens, command);

            if (command.HasSubcommands)
            {
                var subCommands = command.Subcommands;
                for (int i = 0; i < subCommands.Count; i++)
                {
                    AddCommandTokens(tokens, subCommands[i]);
                }
            }

            if (command.HasOptions)
            {
                var options = command.Options;

                for (int i = 0; i < options.Count; i++)
                {
                    AddOptionTokens(tokens, options[i]);
                }
            }

            // TODO: Be sure recursive/global options are handled in the Initialize of Help (add to all)
            return tokens;

            static void AddCommandTokens(Dictionary<string, CliToken> tokens, CliCommand cmd)
            {
                tokens.Add(cmd.Name, Command(cmd.Name, cmd, Location.CreateInternal(cmd.Name.Length)));

                if (cmd._aliases is not null)
                {
                    foreach (string childAlias in cmd._aliases)
                    {
                        tokens.Add(childAlias, Command(childAlias, cmd, Location.CreateInternal(childAlias.Length)));
                    }
                }
            }

            static void AddOptionTokens(Dictionary<string, CliToken> tokens, CliOption option)
            {
                if (!tokens.ContainsKey(option.Name))
                {
                    tokens.Add(option.Name, Option(option.Name, option, Location.CreateInternal(option.Name.Length)));
                }

                if (option._aliases is not null)
                {
                    foreach (string childAlias in option._aliases)
                    {
                        if (!tokens.ContainsKey(childAlias))
                        {
                            tokens.Add(childAlias, Option(childAlias, option, Location.CreateInternal(childAlias.Length)));
                        }
                    }
                }
            }
        }

        private static CliToken GetToken(string? value, CliTokenType tokenType, CliSymbol? symbol, Location location)
            => new(value, tokenType, symbol, location);
        //new Location(argPosition == -1 ? Location.Internal : Location.User,
        //                     argPosition,
        //                     value is null ? 0 : value.Length,
        //                     offset));

        private static CliToken Argument(string arg, Location location)
            => GetToken(arg, CliTokenType.Argument, default, location);

        private static CliToken CommandArgument(string arg, CliCommand command, Location location)
            => GetToken(arg, CliTokenType.Argument, command, location);

        private static CliToken OptionArgument(string arg, CliOption option, Location location)
            => GetToken(arg, CliTokenType.Argument, option, location);

        private static CliToken Command(string arg, CliCommand cmd, Location location)
            => GetToken(arg, CliTokenType.Command, cmd, location);

        private static CliToken Option(string arg, CliOption option, Location location)
            => GetToken(arg, CliTokenType.Option, option, location);

        // TODO: Explore whether double dash should track its command
        private static CliToken DoubleDash(int i, Location location)
            => GetToken(doubleDash, CliTokenType.DoubleDash, default, location);

    }


}