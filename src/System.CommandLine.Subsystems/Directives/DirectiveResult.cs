// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Directives
{
    public class DirectiveResult
    {
        public DirectiveResult(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string? Value { get; }
    }
}
