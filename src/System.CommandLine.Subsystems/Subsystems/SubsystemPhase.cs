﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Subsystems;

// AddSubsystem(CliSubsystem subsystem, SubsystemPhase phase = SubsystemPhase.NotSpecified);

//public enum SubsystemPhase
//{
//    NotSpecified = 0,
//    BeforeDiagram,
//    Diagram,
//    AfterDiagram,
//    BeforeCompletion,
//    Completion,
//    AfterCompletion,
//    BeforeHelp,
//    Help,
//    AfterHelp,
//    BeforeVersion,
//    Version,
//    AfterVersion,
//    BeforeErrorReporting,
//    ErrorReporting,
//    AfterErrorReporting,
//}

// AddSubsystem(CliSubsystem subsystem, SubsystemPhase phase = SubsystemPhase.NotSpecified, PhaseTiming timing = PhaseTiming.Before);

//public enum SubsystemPhase
//{
//    NotSpecified = 0,
//    Diagram,
//    Completion,
//    Help,
//    Version,
//    ErrorReporting,
//}

//public enum PhaseTiming
//{
//    Before = 0,
//    After
//}





///// <summary>
///// Subsystem phases group subsystems that should be run at specific places in CLI processing and 
///// are used for high level ordering.
///// </summary>
///// <remarks>
///// Order of operations:
///// 
///// * Initialize is called for all subsystems, regardless of phase
///// * ExecuteIfNeeded is called for subsystems in the EarlyReturn phase
///// * ExecuteIfNeeded is called for subsystems in the Validate phase
///// * ExecuteIfNeeded is called for subsystems in the Execute phase
///// * ExecuteIfNeeded is called for subsystems in the Finish phase
///// * Teardown is called for all subsystems, regardless of phase
///// </remarks>
//public enum SubsystemPhase
//{
//    /// <summary>
//    /// Indicates a subsystem that never runs, and exists to support other subsystems. ValueSubsystem
//    /// is an example.
//    /// </summary>
//    /// <remarks>
//    /// Initialization runs first, teardown runs last - this is arbitrary and can be changed prior
//    /// to release if we have scenarios to justify.
//    /// </remarks>
//    None,

//    /// <summary>
//    /// Indicates a subsystem is designed to shortcut execution and perform an action other than the 
//    /// action indicated by the command. HelpSubsystem and VersionSubsystem are examples.
//    /// </summary>
//    /// <remarks>
//    /// EarlyReturn subsystems are differentiated from other execution because data validation has not
//    /// occurred. Because of this, data should not be used and should be assume to be questionable.
//    /// </remarks>
//    EarlyReturn,

//    /// <summary>
//    /// Indicates a subsystem that validates data entered by the user. 
//    /// </summary>
//    /// <remarks>
//    /// Errors are not reported, but are rather stored for later display. This may be reconsidered
//    /// if we keep track of which errors have been reported.
//    /// </remarks>
//    Validate,

//    /// <summary>
//    /// Indicates a subsystem that executes using data entered by the user. The only known case is 
//    /// the Invocation subsystem.
//    /// </summary>
//    Execute,

//    /// <summary>
//    /// Indicates a subsystem that runs as the CLI part of processing is ending. ErrorReportingSubsystem
//    /// is an example, although we may rethink when errors are displayed.
//    /// </summary>
//    /// <remarks>
//    /// This is separate from the TearDown step, which is avaiable to all subsystems.
//    /// </remarks>
//    Finish,
//}


