// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Directives;

public class DirectiveOption<T> : CliOption<T>
{
    public static DirectiveOption<T> Create(string name)
    {
        name = name.Replace("[", "")
                   .Replace("]", "");
        var option = new DirectiveOption<T>(name);
        option.Required = false;
        return option;
    }
    private DirectiveOption(string name) : base("[" + name, "[" + name + "]")
    {
            ClosedBy = "]";
    }
}
