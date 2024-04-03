// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;

namespace System.CommandLine.Tests.Utility;

public static class SymbolResultExtensions
{
    public static IEnumerable<string> GetStringTokens(this CommandResult result) 
        => result.ValueResults
            .SelectMany(x => x.Locations.Select(y => y.Text));
}
