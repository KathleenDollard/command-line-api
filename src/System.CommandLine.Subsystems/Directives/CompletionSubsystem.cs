// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;
using System.CommandLine.Subsystems;

namespace System.CommandLine.Directives;

public class CompletionSubsystem(IAnnotationProvider? annotationProvider = null) 
    : DirectiveSubsystem("suggest", SubsystemKind.Completion, annotationProvider)
{
    protected internal override CliExit Execute(PipelineResult pipelineResult)
    {
        var parseResult = pipelineResult.ParseResult;
        string? rawInput = pipelineResult.RawInput;

        int position = !string.IsNullOrEmpty(Value) ? int.Parse(Value) : rawInput?.Length ?? 0;

        var commandLineToComplete = ""; //parseResult.Tokens.LastOrDefault(t => t.Type != CliTokenType.Directive)?.Value ?? "";
        

        var completionParseResult = CliParser.Parse(parseResult.Configuration.RootCommand, commandLineToComplete, parseResult.Configuration);
        //var completionParseResult = parseResult.RootCommandResult.Command.Parse(commandLineToComplete, parseResult.Configuration);

        var completions = GetCompletions(completionParseResult, position);

        pipelineResult.ConsoleHack.WriteLine(
            string.Join(
                Environment.NewLine,
                completions));

        return CliExit.SuccessfullyHandled(pipelineResult.ParseResult);
    }

    
    /// <summary>
    /// Gets completions based on a given parse result.
    /// </summary>
    /// <param name="position">The position at which completions are requested.</param>
    /// <returns>A set of completions for completion.</returns>
    public IEnumerable<CompletionItem> GetCompletions(ParseResult parseResult,
        int? position = null)
    {
        SymbolResult currentSymbolResult = SymbolToComplete(position);

        CliSymbol currentSymbol = currentSymbolResult switch
        {
            ArgumentResult argumentResult => argumentResult.Argument,
            OptionResult optionResult => optionResult.Option,
            DirectiveResult directiveResult => directiveResult.Directive,
            _ => ((CommandResult)currentSymbolResult).Command
        };

        var context = GetCompletionContext();

        if (position is not null &&
            context is TextCompletionContext tcc)
        {
            context = tcc.AtCursorPosition(position.Value);
        }

        var completions = currentSymbol.GetCompletions(context);

        string[] optionsWithArgumentLimitReached = currentSymbolResult is CommandResult commandResult
                                                        ? OptionsWithArgumentLimitReached(commandResult)
                                                        : Array.Empty<string>();

        completions =
            completions.Where(item => optionsWithArgumentLimitReached.All(s => s != item.Label));

        return completions;

        static string[] OptionsWithArgumentLimitReached(CommandResult commandResult) =>
            commandResult
                .Children
                .OfType<OptionResult>()
                .Where(c => c.IsArgumentLimitReached)
                .Select(o => o.Option)
                .SelectMany(c => new[] { c.Name }.Concat(c.Aliases))
                .ToArray();
    }
}
