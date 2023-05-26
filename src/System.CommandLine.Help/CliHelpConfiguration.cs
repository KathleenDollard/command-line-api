namespace System.CommandLine.Help
{
    public class CliHelpConfiguration
    {
        private CliHelpLayout? currentLayout;

        public CliHelpConfiguration(CliHelpFormatting? helpFormatting = null, 
            CliHelpSymbolOutput? symbolOutput = null, int maxWidth = int.MaxValue)
        {
            HelpFormatting = helpFormatting ?? new CliHelpFormatting();
            SymbolOutput = symbolOutput ?? new CliHelpSymbolOutput(this );
        }

        //public Dictionary<string, CliHelpLayout> Layouts => new();
        public CliHelpLayout CurrentLayout
        {
            get => currentLayout ?? new CliHelpLayout(this); 
            set => currentLayout = value;
        }
        public CliHelpFormatting HelpFormatting { get; set; }
        public CliHelpSymbolOutput SymbolOutput { get; }
    }
}