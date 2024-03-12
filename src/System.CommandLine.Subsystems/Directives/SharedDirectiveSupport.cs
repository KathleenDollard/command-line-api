// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
/*
using System.CommandLine.Subsystems;

namespace System.CommandLine.Directives;

/// <summary>
/// Provides helper methods for [..] style directives and allows caching of directive evaluation
/// </summary>
public class SharedDirectiveSupport
{
    private List<DirectiveResult>? results = null;

    private IEnumerable<DirectiveResult> GetResults(CliConfiguration configuration, IReadOnlyList<string> args)
    {
        if (results is null)
        {
            results = ParseForDirectives(configuration, args);
        }
        return results;

        static List<DirectiveResult> ParseForDirectives(CliConfiguration configuration, IReadOnlyList<string> args)
        {
            var foundEndOfDirectives = false;
            var startPosition = configuration.FirstArgumentIsRootCommand(args) ? 1 : 0;
            var localResults = new List<DirectiveResult>();

            for (var i = startPosition; i < args.Count; i++)
            {
                var arg = args[i];

                if (!foundEndOfDirectives)
                {
                    // The following pattern should be equivalent to previous arg.Length > 2 && arg[0] == '[' && arg[1] != ']' && arg[1] != ':' && arg[arg.Length - 1] == ']'
                    if (arg is ['[', .. var strippedArg, ']'])
                    {
                        localResults.Add(CreateDirective(strippedArg));
                    }
                    else
                    {
                        // Changed logic here to just quit when they no longer match
                        break;
                        //if (!configuration.RootCommand.EqualsNameOrAlias(arg))
                        //{
                        //    foundEndOfDirectives = true;
                        //}
                    }
                }
            }
            return localResults;
        }

        static DirectiveResult CreateDirective(string arg)
        {
            // Trusting the BCL is faster than me
            var split = arg.Split(':');
            int colonIndex = arg.AsSpan().IndexOf(':');

            string name = colonIndex > 0
                ? arg.Substring(0, colonIndex) // [name:value]
                : arg; // [name] is a legal directive
            string? value = colonIndex > 0
                ? arg.Substring(colonIndex+ 1)
                : null;

            return new DirectiveResult(name, value);
            // Changing logic. Directives are not int the CLI declaration, so they can't be in knownTokens
            //if (knownTokens.TryGetValue($"[{directiveName}]", out var directiveToken))
            //{
            //    directive = (CliDirective)directiveToken.Symbol!;
            //}
            //else
            //{
            //    directive = null;
            //}

            //tokenList.Add(Directive(arg, directive));
        }
    }

    public IEnumerable<DirectiveResult> FindDirective(string directiveName, InitializationContext initializationContext)
    {
        return GetResults(initializationContext.Configuration, initializationContext.Args).Where(x => x.Name == directiveName);
    }
}
*/