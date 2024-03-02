﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Subsystem.Annotations;
using System.Reflection;

namespace System.CommandLine.Subsystem
{
    public class Version : CliSubsystem
    {
        private string specificVersion = null;

        public Version(IAnnotationProvider? annotationProvider = null)
            : base("Version", annotationProvider, SubsystemKind.Version)
        {
        }


        // TODO: Should we block adding version anywhere but root?
        public string SpecificVersion
        {
            get
            {
                var version = specificVersion is null
                    ? AssemblyVersion(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                    : specificVersion;
                return version ?? "";
            }
            set => specificVersion = value;
        }

        public static string? AssemblyVersion(Assembly assembly) 
            => assembly is null
                ? null
                : assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;


        protected internal override bool Initialize(CliConfiguration configuration)
        {
            var option = new CliOption<bool>("--version", ["-v"])
            {
                Arity = ArgumentArity.Zero
            };
            configuration.RootCommand.Add(option);

            return true;
        }

        // TODO: Stash option rather than using string
        protected internal override bool GetIsActivated(ParseResult parseResult)
            => parseResult.GetValue<bool>("--version");

        protected internal override CliExit Execute(PipelineContext pipelineContext)
        {
            var subsystemVersion = SpecificVersion;
            var version = subsystemVersion is null
                ? CliExecutable.ExecutableVersion
                : subsystemVersion;
            pipelineContext.ConsoleHack.WriteLine(version);
            pipelineContext.AlreadyHandled = true;
            return CliExit.SuccessfullyHandled(pipelineContext.ParseResult);
        }
    }
}
