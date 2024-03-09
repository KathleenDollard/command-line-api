﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Directives;
using System.CommandLine.Parsing;
using System.CommandLine.Subsystems;

namespace System.CommandLine;

public class Pipeline
{
    protected SharedDirectiveSupport SharedDirectiveSupport { get; } = new();

    public HelpSubsystem? Help { get; set; }
    public VersionSubsystem? Version { get; set; }
    public CompletionSubsystem? Completion { get; set; }
    public DiagramSubsystem? Diagram { get; set; }
    public ErrorReportingSubsystem? ErrorReporting { get; set; }

    public ParseResult Parse(CliConfiguration configuration, string rawInput)
        => Parse(configuration, CliParser.SplitCommandLine(rawInput).ToArray());

    public ParseResult Parse(CliConfiguration configuration, IReadOnlyList<string> args)
    {
        InitializeSubsystems(configuration, args);
        var parseResult = CliParser.Parse(configuration.RootCommand, args, configuration);
        return parseResult;
    }

    public CliExit Execute(CliConfiguration configuration, string rawInput, ConsoleHack? consoleHack = null)
        => Execute(configuration, CliParser.SplitCommandLine(rawInput).ToArray(), rawInput, consoleHack);

    public CliExit Execute(CliConfiguration configuration, string[] args, string rawInput, ConsoleHack? consoleHack = null)
    {
        var cliExit = Execute(Parse(configuration, args), rawInput, consoleHack);
        return TearDownSubsystems(cliExit);
    }

    public CliExit Execute(ParseResult parseResult, string rawInput, ConsoleHack? consoleHack = null)
    {
        var pipelineContext = new PipelineContext(parseResult, rawInput, this, consoleHack ?? new ConsoleHack());
        ExecuteSubsystems(pipelineContext);
        return new CliExit(pipelineContext);
    }

    protected virtual void InitializeHelp(CliConfiguration configuration, IReadOnlyList<string> args)
        => Help?.Initialize(configuration, args);

    protected virtual void InitializeVersion(CliConfiguration configuration, IReadOnlyList<string> args)
        => Version?.Initialize(configuration, args);

    protected virtual void InitializeCompletion(CliConfiguration configuration, IReadOnlyList<string> args)
        => Completion?.Initialize(configuration, args);

    protected virtual void InitializeDiagram(CliConfiguration configuration, IReadOnlyList<string> args)
    => Diagram?.Initialize(configuration, args);

    protected virtual void InitializeErrorReporting(CliConfiguration configuration, IReadOnlyList<string> args)
        => ErrorReporting?.Initialize(configuration, args);

    protected virtual CliExit TearDownHelp(CliExit cliExit)
        => Help is null
                ? cliExit
                : Help.TearDown(cliExit);

    protected virtual CliExit? TearDownVersion(CliExit cliExit)
        => Version is null
                ? cliExit
                : Version.TearDown(cliExit);

    protected virtual CliExit TearDownCompletion(CliExit cliExit)
        => Completion is null
                ? cliExit
                : Completion.TearDown(cliExit);

    protected virtual CliExit TearDownDiagram(CliExit cliExit)
        => Diagram is null
                ? cliExit
                : Diagram.TearDown(cliExit);

    protected virtual CliExit TearDownErrorReporting(CliExit cliExit)
        => ErrorReporting is null
                ? cliExit
                : ErrorReporting.TearDown(cliExit);

    protected virtual void ExecuteHelp(PipelineContext context)
        => ExecuteIfNeeded(Help, context);

    protected virtual void ExecuteVersion(PipelineContext context)
        => ExecuteIfNeeded(Version, context);

    protected virtual void ExecuteCompletion(PipelineContext context)
        => ExecuteIfNeeded(Completion, context);

    protected virtual void ExecuteDiagram(PipelineContext context)
    => ExecuteIfNeeded(Diagram, context);

    protected virtual void ExecuteErrorReporting(PipelineContext context)
        => ExecuteIfNeeded(ErrorReporting, context);

    // TODO: Consider whether this should be public. It would simplify testing, but would it do anything else
    // TODO: Confirm that it is OK for ConsoleHack to be unavailable in Initialize
    /// <summary>
    /// Perform any setup for the subsystem. This may include adding to the CLI definition,
    /// such as adding a help option. It is important that work only needed when the subsystem
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <remarks>
    /// Note to inheritors: The ordering of initializing should normally be in the reverse order than tear down 
    /// </remarks>
    protected virtual void InitializeSubsystems(CliConfiguration configuration, IReadOnlyList<string> args)
    {
        InitializeHelp(configuration, args);
        InitializeVersion(configuration, args);
        InitializeCompletion(configuration, args);
        InitializeDiagram(configuration, args);
        InitializeErrorReporting(configuration, args);
    }

    // TODO: Consider whether this should be public
    // TODO: Would Dispose be a better alternative? This may be non-dispose like things, such as removing options?
    /// <summary>
    /// Perform any cleanup operations
    /// </summary>
    /// <param name="pipelineContext">The context of the current execution</param>
    /// <remarks>
    /// Note to inheritors: The ordering of tear down should normally be in the reverse order than initializing
    /// </remarks>
    protected virtual CliExit TearDownSubsystems(CliExit cliExit)
    {
        TearDownErrorReporting(cliExit);
        TearDownDiagram(cliExit);
        TearDownCompletion(cliExit);
        TearDownVersion(cliExit);
        TearDownHelp(cliExit);
        return cliExit;
    }

    protected virtual void ExecuteSubsystems(PipelineContext pipelineContext)
    {
        ExecuteHelp(pipelineContext);
        ExecuteVersion(pipelineContext);
        ExecuteCompletion(pipelineContext);
        ExecuteDiagram(pipelineContext);
        ExecuteErrorReporting(pipelineContext);
    }

    protected static void ExecuteIfNeeded(CliSubsystem? subsystem, PipelineContext pipelineContext)
    {
        if (subsystem is not null && (!pipelineContext.AlreadyHandled || subsystem.RunsEvenIfAlreadyHandled))
        {
            subsystem.ExecuteIfNeeded(pipelineContext);
        }
    }

}
