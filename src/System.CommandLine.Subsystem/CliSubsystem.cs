// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Extended.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace System.CommandLine.Subsystem;

/// <summary>
/// Base class for CLI subsystems. Implements storage of annotations.
/// </summary>
/// <remarks>
/// annotationProvider is required because deriving types should accept that parameter and pass it in almost all cases
/// </remarks>
/// <param name="annotationProvider"></param>
public abstract class CliSubsystem(string name, PipelineSupport pipelineSupport, IAnnotationProvider? annotationProvider)

{

    /// <summary>
    /// The name of the extension. 
    /// </summary>
    public string Name { get; } = name;

    public PipelineSupport PipelineSupport { get; protected set; } = pipelineSupport;

    DefaultAnnotationProvider? _defaultProvider;
    readonly IAnnotationProvider? _annotationProvider = annotationProvider;

    protected internal bool TryGetAnnotation<TValue>(CliSymbol symbol, AnnotationId<TValue> id, [NotNullWhen(true)] out TValue? value)
    {
        if (_defaultProvider is not null && _defaultProvider.TryGet(symbol, id, out value))
        {
            return true;
        }
        if (_annotationProvider is not null && _annotationProvider.TryGet(symbol, id, out value))
        {
            return true;
        }
        value = default;
        return false;
    }

    // TODO: Consider allowing null so the annotation provider can hold general information. Alternatively, it would be stored in the subsystem itself.
    protected internal void SetAnnotation<T>(CliSymbol symbol, AnnotationId<T> id, T value)
    {
        (_defaultProvider ??= new DefaultAnnotationProvider()).Set(symbol, id, value);
    }
}