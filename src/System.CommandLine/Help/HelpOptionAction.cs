using System.Threading;
using System.Threading.Tasks;

namespace System.CommandLine.Help
{
    public sealed class HelpAction : CliAction
    {
        private IHelpBuilder? _builder;

        /// <summary>
        /// Specifies an <see cref="Builder"/> to be used to format help output when help is requested.
        /// </summary>
        public IHelpBuilder Builder
        {
            get => _builder ??= new HelpBuilderOld();
            set => _builder = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override int Invoke(ParseResult parseResult)
        {
            var output = parseResult.Configuration.Output;

            var helpContext = new HelpContext(parseResult.CommandResult.Command,
                                              Console.IsOutputRedirected ? int.MaxValue : Console.WindowWidth,
                                              output,
                                              parseResult);

            Builder.Write(helpContext);

            return 0;
        }

        public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
            => cancellationToken.IsCancellationRequested
                ? Task.FromCanceled<int>(cancellationToken)
                : Task.FromResult(Invoke(parseResult));
    }
}
