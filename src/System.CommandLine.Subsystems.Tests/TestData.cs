// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Reflection;

namespace System.CommandLine.Subsystems.Tests;

internal class TestData
{
    internal static readonly string? AssemblyVersionString = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                                     ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                     ?.InformationalVersion;

    internal class Version : IEnumerable<object[]>
    {
        private readonly List<object[]> _data =
        [
            ["--version", true],
            ["-v", true],
            ["-vx", true],
            ["-xv", true],
            ["-x", false],
            [null, false],
            ["", false],
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class Diagram : IEnumerable<object[]>
    {
        private readonly List<object[]> _data =
        [
            ["[Diagram]", true, 1],
            ["[Diagram:hello]", true, 1],
            ["[Diagram] x", true, 1],
            ["[Diagram] -o", true, 1],
            ["[Diagram] -v", true, 1],
            ["[Diagram] x -v", true,1 ],
            ["[DiagramX]", false, 1],
            ["[Diagram] [Other]", true, 2],
            ["[Diagram:hello] [Other] [AndAlso]", true, 3],
            ["x", false,0],
            ["-o", false,0],
            ["x -x", false,0],
            [null, false,0],
            ["", false, 0]
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}
