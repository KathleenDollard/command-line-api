// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.CommandLine.Parsing;

public class CommandValueResult
{
    public CommandValueResult(CommandResult commandResult, Location location)
    {
        var valueResults = new List<ValueResult>();
        foreach (var child in commandResult.Children)
        {
            if (child is OptionResult optionResult)
            {
                valueResults.Add(optionResult.ValueResult);
            }
            if (child is ArgumentResult argumentResult)
            {
                valueResults.Add(argumentResult.ValueResult);
            }
        }
        Location = location;
        ValueResults = valueResults;
    }

    public IEnumerable<ValueResult> ValueResults { get; }
    public Location Location { get; }
}
