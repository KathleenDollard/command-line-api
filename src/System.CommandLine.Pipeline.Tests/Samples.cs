// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.CommandLine.Parsing;
using System.CommandLine.Pipeline;
using Xunit;

namespace System.CommandLine.Extended.Tests
{
    public class Samples
    {

        [Fact]
        [Trait("Category", "Sample")]
        public void Author_can_run_with_just_the_parser()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var result = CliParser.Parse(configuration.RootCommand, "");

            result.Should().NotBeNull();
        }

        [Fact]
        [Trait("Category", "Sample")]
        // NOTE: This test is a little gnarly because we anticipate users that want to add extensions will run them via the pipeline
        public void Author_can_add_subsystem_and_manually_handle()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            var pipeline = new Pipeline.Pipeline();
            pipeline.AddExtension(new VersionExtension.VersionExtension());

            var parseResult = pipeline.Parse(configuration, "");
            if (versionOption.PipelineExtension.GetIsActivated(parseResult))
            {
                Console.WriteLine("Version!!!!");
            }

            versionOption.PipelineExtension.TempFlagForTest.Should().BeTrue();
        }

        //[Fact]
        //[Trait("Category", "Sample")]
        //// TODO: This test is requires solving where the standard extensions should be located without a circular dependency
        //public void Author_can_add_standard_set_of_subsystems_and_manually_execute()
        //{
        //    var rootCommand = new CliRootCommand
        //    { };
        //    var configuration = new CliConfiguration(rootCommand);
        //    var pipeline = new Pipeline.Pipeline().AddStandardExtensions();

        //    var parseResult = pipeline.Parse(configuration, "");
        //    PipelineResult? pipelineResult = null;
        //    if (versionOption.PipelineExtension.GetIsActivated(parseResult))
        //    {
        //        pipelineResult = versionOption.PipelineExtension.Execute(parseResult);
        //    }
        //    else if (helpOption.PipelineExtension.GetIsActivated(parseResult))
        //    {
        //        pipelineResult = versionOption.PipelineExtension.Execute(parseResult);
        //    }

        //    pipelineResult.Handled.Should().BeTrue();
        //    var versionOption = pipeline.GetExtensions().Where(x=>x.Name=="VersionOption");
        //    versionOption.PipelineExtension.TempFlagForTest.Should().BeTrue();
        //}

        [Fact]
        [Trait("Category", "Sample")]
        public void Author_can_add_one_subsystem_and_auto_execute_them()
        {
            var rootCommand = new CliRootCommand
            { };
            var configuration = new CliConfiguration(rootCommand);
            var versionOption = new VersionExtension.VersionExtension();
            //TODO: Consider whether this should allow fluent for adding extensions
            var pipeline = new Pipeline.Pipeline();
            pipeline.AddExtension(new VersionExtension.VersionExtension());

            var pipelineResult = pipeline.Execute(configuration,"");

            pipelineResult.Handled.Should().BeTrue();
            versionOption.PipelineExtension.TempFlagForTest.Should().BeTrue();
        }

        //[Fact]
        //[Trait("Category", "Sample")]
        ////// TODO: This test is requires solving where the standard extensions should be located without a circular dependency
        //public void Author_can_add_standard_set_of_subsystems_and_auto_execute_them()
        //{
        //    var rootCommand = new CliRootCommand
        //    { };
        //    var configuration = new CliConfiguration(rootCommand);
        //    var pipeline = new Pipeline.Pipeline().AddStandardExtensions();

        //    var pipelineResult = pipeline.Execute(configuration, "");

        //    pipelineResult.Handled.Should().BeTrue();
        //    var versionOption = pipeline.GetExtensions().Where(x=>x.Name=="VersionOption");
        //    versionOption.PipelineExtension.TempFlagForTest.Should().BeTrue();
        //}

        //[Fact]
        //[Trait("Category", "Sample")]
        //public void Author_can_add_subsystem_and_invoke_command()
        //{ }

        //[Fact]
        //[Trait("Category", "Sample")]
        //public void Author_can_add_subsystem_and_aut_execute_instead_of_invoking()
        //{ }



    }
}