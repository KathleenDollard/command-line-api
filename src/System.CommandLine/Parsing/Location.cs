// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.CommandLine.Parsing
{
    public record Location
    {
        public const string Implicit = "Implicit";
        public const string Internal = "Internal";
        public const string User = "User";
        public const string Response = "Response";

        internal static Location CreateImplicit(int length, Location? outerLocation = null, int offset = 0)
           => new(Implicit, -1, length, offset, outerLocation);
        internal static Location CreateInternal(int length, Location? outerLocation = null, int offset = 0)
           => new(Internal, -1, length, offset, outerLocation);
        internal static Location CreateUser(int start, int length, Location? outerLocation = null, int offset = 0) 
            => new Location(User, start, length, offset);
        internal static Location CreateResponse(string responseSourceName, int start, int length, Location? outerLocation = null, int offset = 0)
            => new Location($"{Response}:{responseSourceName}", start, length, offset);

        
        internal static Location FromOuterLocation(int start, int length, Location outerLocation, int offset = 0) 
            => new(outerLocation.Source, start, length, offset, outerLocation);

        public Location(string source, int start, int length, int offset = 0, Location? outerLocation = null)
        {
            Source = source;
            Start = start;
            Length = length;
            Offset = offset;
            OuterLocation = outerLocation;
        }

        public string Source { get; }
        public int Start { get; }
        public int Offset { get; }
        public int Length { get; }
        public Location? OuterLocation { get; }

        public bool IsImplicit
            => Source == Implicit;

        public override string ToString() 
            => $"{(OuterLocation is null ? "" : OuterLocation.ToString() + "; ")}{Source} [{Start}, {Length}, {Offset}]";
    }
}