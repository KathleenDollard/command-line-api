// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.CommandLine.Subsystem.Help
{
    public class HelpPipelineSupport : PipelineSupport
    {
        public HelpPipelineSupport(): base(CategoryAfterValidation)
        { }


        public override bool Initialization(CliConfiguration configuration, IReadOnlyList<string> arguments, string rawInput)
        {
            var option = new CliOption<bool>("--help", ["-h"])
            {
                Arity = ArgumentArity.Zero
            };
            configuration.RootCommand.Add(option);

            return true;
        }

        public override bool GetIsActivated(ParseResult  parseResult)
            => parseResult.GetValue<bool>("--help");

        public override CliExit Execute(PipelineContext pipelineContext)
        {
            // TODO: Match testable output pattern
            pipelineContext.ConsoleHack.WriteLine("Help me!");
            return new CliExit(pipelineContext.ParseResult, true, 0);
        }
    }
}
