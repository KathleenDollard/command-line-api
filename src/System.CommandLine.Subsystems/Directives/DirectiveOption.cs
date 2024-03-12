// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Directives
{
    public class DirectiveOption<T> : CliOption<T>
    {
        public DirectiveOption(string name) : base("[" + name)
        {
            ClosedBy = "]"; 
        }
    }
}
