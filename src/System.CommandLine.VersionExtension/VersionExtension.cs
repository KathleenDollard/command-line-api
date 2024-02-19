﻿using System.CommandLine.Extensions;

namespace System.CommandLine.VersionExtension
{
    // TODO: Remove the duplication in System.CommandLine.VersionExtension.VersionExtension
    public class VersionExtension : Extension
    {
        public VersionExtension() : base("Version", CategoryAfterValidation)
        { }

        public override bool BeforeParsing(CliCommand rootCommand, IReadOnlyList<string> arguments, string rawInput, CliConfiguration configuration)
        {
            var option = new CliOption<bool>("--version", ["-v"])
            {
                Arity = ArgumentArity.Zero
            };
            rootCommand.Add(option);

            return true;
        }

        public override bool Execute(ParseResult result)
        {
            // TODO: Match testable output pattern
            Console.WriteLine(CliExecutable.ExecutableVersion);
            return true;
        }
    }
}
