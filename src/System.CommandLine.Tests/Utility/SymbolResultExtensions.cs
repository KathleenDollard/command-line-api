// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Parsing;

namespace System.CommandLine.Tests.Utility;

public static class SymbolResultExtensions
{
    public IEnumerable<string> GetStringTokens(this SymbolResult result)
    {
        result
                  .Tokens
                  .Select(t => t.Value)
    }
}
