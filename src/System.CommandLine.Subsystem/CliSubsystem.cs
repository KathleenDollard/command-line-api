// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Extended.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.PortableExecutable;

namespace System.CommandLine.Subsystem;

public abstract class CliSubsystem
{
    protected CliSubsystem(string name, PipelineSupport pipelineSupport, IAnnotationProvider? annotationProvider)
    {
        Name = name;
        PipelineSupport = pipelineSupport;
        _annotationProvider = annotationProvider;
    }

    public PipelineSupport PipelineSupport { get; }

    /// <summary>
    /// The name of the extension. 
    /// </summary>
    public string Name { get; }

    DefaultAnnotationProvider? _defaultProvider;
    readonly IAnnotationProvider? _annotationProvider;

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


/// <summary>
/// Base class for CLI subsystems. Implements storage of annotations.
/// </summary>
/// <remarks>
/// annotationProvider is required because deriving types should accept that parameter and pass it in almost all cases
/// </remarks>
public abstract class CliSubsystem<TSubsystem> : CliSubsystem
    where TSubsystem : CliSubsystem<TSubsystem>
{
    /// <param name="annotationProvider"></param>
    public CliSubsystem(string name, PipelineSupport<TSubsystem> pipelineSupport, IAnnotationProvider? annotationProvider)
        : base(name, pipelineSupport, annotationProvider)
    {
        PipelineSupport.Subsystem = (TSubsystem)this;
    }

    public new PipelineSupport<TSubsystem> PipelineSupport => (PipelineSupport<TSubsystem>)base.PipelineSupport;
}

