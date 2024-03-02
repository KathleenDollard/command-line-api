﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystem;

/// <summary>
/// Describes the ID and type of an annotation.
/// </summary>
public record struct AnnotationId<TValue>(string Id);
