﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
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
            ["[diagram]", true],
            ["[diagram:hello]", true],
            ["[diagram] x", true],
            ["[diagram] -o", true],
            ["[diagram] -v", true],
            ["[diagram] x -v", true ],
            ["[diagramX]", false],
            ["[diagram] [other]", true],
            ["[diagram:hello] [other] [and-also]", true],
            ["x", false],
            ["-o", false],
            ["x -x", false],
            [null, false],
            ["", false]
        ];

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}
