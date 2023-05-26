namespace System.CommandLine.Help
{
    public class CliHelpConfiguration
    {
        private CliHelpLayout? currentLayout;

        public CliHelpConfiguration(CliHelpSymbolOutput? symbolOutput = null, int indent = 0)
        {
            SymbolOutput = symbolOutput ?? new CliHelpSymbolOutput(this );
            Indent = indent ==0 ? 2:indent;
        }

        //public Dictionary<string, CliHelpLayout> Layouts => new();
        public CliHelpLayout CurrentLayout
        {
            get => currentLayout ?? new CliHelpLayout(this); 
            set => currentLayout = value;
        }
        public CliHelpSymbolOutput SymbolOutput { get; }
        public int Indent { get; }
    }
}