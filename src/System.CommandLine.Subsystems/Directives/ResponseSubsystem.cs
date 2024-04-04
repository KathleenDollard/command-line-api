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
        context.Configuration.ResponseFileTokenReplacer = DefaultTokenReplacer;
        return context.Configuration;
    }

    public static (List<string>? tokens, List<string>? errors) DefaultTokenReplacer(string responseSourceName)
    {
        try
        {
            return TryReadResponseFile(responseSourceName, out var newTokens, out var errors)
                ? (newTokens, null)
                : (null, errors);
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

    internal static bool TryReadResponseFile(
         string filePath,
         out List<string>? newTokens,
         out List<string>? errors)
    {
        try
        {
            newTokens = ExpandResponseFile(filePath).ToList();
            errors = null;
            return true;
        }
        catch (FileNotFoundException)
        {

            errors = new() { LocalizationResources.ResponseFileNotFound(filePath) };

        }
        catch (IOException e)
        {
            errors = new() { LocalizationResources.ErrorReadingResponseFile(filePath, e) };
        }

        newTokens = null;
        return false;
    }
    private static IEnumerable<string> ExpandResponseFile(string filePath)
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

    private static IEnumerable<string> SplitLine(string line)
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

    private static string? GetReplaceableTokenValue(string arg) =>
    arg.Length > 1 && arg[0] == '@'
        ? arg.Substring(1)
        : null;


}