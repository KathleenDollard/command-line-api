// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;
using System.CommandLine.Subsystems;

namespace System.CommandLine.Directives;

public class ResponseSubsystem()
    : CliSubsystem("Response", SubsystemKind.Response, null)
{
    protected internal override CliConfiguration Initialize(InitializationContext context)
    {
        context.Configuration.ResponseFileTokenReplacer = Replacer;
        return context.Configuration;
    }

    public static (List<string>? tokens, List<string>? errors) Replacer(string responseSourceName)
    {
        try
        {
            var contents = File.ReadAllText(responseSourceName);
            return (CliParser.SplitCommandLine(contents).ToList(), null);
        }
        catch
        {
            // TODO: Switch to proper errors
            return (null, 
                    errors:
                    [
                        $"Failed to open response file {responseSourceName}"
                    ]);
        }
    }
}