﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace System.CommandLine.Subsystems;

public abstract class CliSubsystem
{
    protected CliSubsystem(string name, IAnnotationProvider? annotationProvider, SubsystemKind subsystemKind)
    {
        Name = name;
        _annotationProvider = annotationProvider;
        SubsystemKind = subsystemKind;
    }

    /// <summary>
    /// The name of the extension. 
    /// </summary>
    public string Name { get; }
    public SubsystemKind SubsystemKind { get; }

    DefaultAnnotationProvider? _defaultProvider;
    readonly IAnnotationProvider? _annotationProvider;

    /// <summary>
    /// Attempt to retrieve the value for the symbol and annotation ID from the annotation provider. 
    /// </summary>
    /// <typeparam name="TValue">The value of the type to retrieve</typeparam>
    /// <param name="symbol">The symbol the value is attached to</param>
    /// <param name="id">The id for the value to be retrieved. For example, an annotation ID for help is description</param>
    /// <param name="value">An out parameter to contain the result</param>
    /// <returns>True if successful</returns>
    /// <remarks>
    /// This value is protected because these values are always retrieved from derived classes that offer 
    /// strongly typed explicit methods, such as help having `GetDescription(Symbol symbol)` method.
    /// </remarks>
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

    /// <summary>
    /// Set the value for the symbol and annotation ID in the annotation provider.
    /// </summary>
    /// <typeparam name="TValue">The value of the type to set</typeparam>
    /// <param name="symbol">The symbol the value is attached to</param>
    /// <param name="id">The id for the value to be set. For example, an annotation ID for help is description</param>
    /// <param name="value">An out parameter to contain the result</param>
    /// <remarks>
    /// This value is protected because these values are always retrieved from derived classes that offer 
    /// strongly typed explicit methods, such as help having `GAetDescription(Symbol symbol, "My help descrption")` method.
    /// </remarks>
    protected internal void SetAnnotation<TValue>(CliSymbol symbol, AnnotationId<TValue> id, TValue value)
    {
        (_defaultProvider ??= new DefaultAnnotationProvider()).Set(symbol, id, value);
    }

    protected internal virtual bool RunsEvenIfAlreadyHandled { get; protected set; }

    /// <summary>
    /// Executes the behavior of the subsystem. For example, help would write information to the console.
    /// </summary>
    /// <param name="pipelineContext">The context contains data like the ParseResult, and allows setting of values like whether execution was handled and the CLI should terminate </param>
    /// <returns>A CliExit object with information such as whether the CLI should terminate</returns>
    protected internal virtual CliExit Execute(PipelineContext pipelineContext) => CliExit.NotRun(pipelineContext.ParseResult);

     internal PipelineContext ExecuteIfNeeded(PipelineContext pipelineContext)
        => ExecuteIfNeeded(pipelineContext.ParseResult,  pipelineContext);

    internal PipelineContext ExecuteIfNeeded(ParseResult? parseResult, PipelineContext pipelineContext)
    {
        if( GetIsActivated(parseResult))
        {
            Execute(pipelineContext );
        }
        return pipelineContext;
    }


    /// <summary>
    /// Indicates to invocation patterns that the extension should be run.
    /// </summary>
    /// <remarks>
    /// This may be explicitly set, such as a directive like Diagram, or it may explore the result
    /// </remarks>
    /// <param name="result">The parse result.</param>
    /// <returns></returns>
    protected internal virtual bool GetIsActivated(ParseResult? parseResult) => false;

    /// <summary>
    /// Runs before parsing to prepare the parser. Since it always runs, slow code that is only needed when the extension 
    /// runs as part of invocation should be delayed to BeforeRun(). Default behavior is to do nothing.
    /// </summary>
    /// <remarks>
    /// Use cases:
    /// * Add to the CLI, such as adding version option
    /// * Early setup of extension internal data, such as reading a file that contains defaults
    /// * Licensing if early exit is needed
    /// </remarks>
    /// <param name="configuration">The CLI configuration, which contains the RootCommand for customization</param>
    /// <returns>True if parsing should continue</returns> // there might be a better design that supports a message
    // TODO: Because of this and similar usage, consider combining CLI declaration and config. ArgParse calls this the parser, which I like
    protected internal virtual CliConfiguration Initialize(CliConfiguration configuration) => configuration;

    // TODO: Determine if this is needed.
    protected internal virtual CliExit TearDown(CliExit cliExit) 
        => cliExit;

}



