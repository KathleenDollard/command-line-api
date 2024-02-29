// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Extended.Annotations;
using System.Reflection;

namespace System.CommandLine.Subsystem
{
    public class Version : CliSubsystem<Version>
    {
        private string specificVersion = null;

        public Version(IAnnotationProvider? annotationProvider = null) 
            : base( "Version",  new VersionPipelineSupport(), annotationProvider)
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

    }
}
