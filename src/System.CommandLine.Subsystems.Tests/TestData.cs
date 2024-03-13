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
            ["[diagram]", true, "My Value"],
            ["[diagram:Hello]", true, "Hello"],
            ["[diagram] x", true, "My Value"],
            ["[diagram] -o", true, "My Value"],
            ["[diagram] -v", true, "My Value"],
            ["[diagram] x -v", true , "My Value"],
            ["[diagramX]", false, null],
            ["[diagram] [other]", true, null],
            ["[diagram:Hello] [other] [and-also]", true, "Hello"],
            ["x", false, null],
            ["-o", false, null],
            ["x -x", false, null],
            [null, false, null],
            ["", false, null]
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}
