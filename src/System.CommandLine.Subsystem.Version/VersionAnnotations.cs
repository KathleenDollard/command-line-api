// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Extended.Annotations;

/// <summary>
/// IDs for well-known Version annotations.
/// </summary>
public static class VersionAnnotations
{
    const string Prefix = "Version.";
    public static AnnotationId<string> Description { get; } = new(Prefix + nameof(Description));
}
