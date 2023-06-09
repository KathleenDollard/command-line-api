using System.CommandLine.CliOutput;

namespace System.CommandLine.Help
{
    public class HelpConfiguration :CliSpecificConfiguration
    {
        public const string Key = "help";
        public HelpConfiguration()
            : base(Key) 
        {
            GetSymbolInspector = helpContext => new CliSymbolInspector();
        }

        public CliFormatter? Formatter { get;set; }

        public Func<HelpContext, CliSymbolInspector> GetSymbolInspector { get; set; }

        public override void AddSymbol(CliCommand rootCommand)
        {
            rootCommand.Add(new HelpOption());
        }
    }
}
