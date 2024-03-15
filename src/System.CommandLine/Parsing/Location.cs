// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.CommandLine.Parsing
{
    public record Location
    {
        public const string Implicit = "Implicit";
        public const string Internal = "Internal";
        public const string User = "User";

        internal static Location CreateImplicit(int length)
           => new(Implicit, -1, length);
        internal static Location CreateInternal(int length)
           => new(Internal, -1, length);

        public Location(string source, int start, int length, int offset)
        {
            Source = source;
            Start = start;
            Offset = offset;
            Length = length;
        }

        public Location(string source, int start, int length)
        {
            Source = source;
            Start = start;
            Length = length;
            Offset = 0;
        }

        public static Location[] CreateList(string source, IReadOnlyList<string> args)
        {
            var ret = new Location[args.Count];
            for (int i = 0; i < args.Count; i++)
            {
                ret[i] = new Location(source, i, args[i].Length, 0);
            }
            return ret;
        }



        public string Source { get; }
        public int Start {  get; }
        public int Offset {  get; }
        public int Length { get; }

        public bool IsImplicit
            => Source == Implicit;
    }
}