using System.CommandLine.CliOutput;

namespace System.CommandLine.Help
{
    public static class CliHelpUtilities
    { 
        // KAD: perf question: Is it worth the statics here for the caching. We cannot guarantee that 
        //      GetSymbolInspector will be fast, and this is called about 6 times for every call to help
        private static HelpContext? previousHelpContext = null;
        private static CliSymbolInspector? symbolInspector = null;

        public static CliSymbolInspector SymbolInspector(HelpContext helpContext)
        {
            if (helpContext != previousHelpContext || symbolInspector is null)
            {
                previousHelpContext = helpContext;
                symbolInspector = previousHelpContext is null
                    ? throw new ArgumentException("Argument should be a HelpContext", nameof(helpContext))
                    : previousHelpContext.CliConfiguration.HelpConfiguration.GetSymbolInspector(previousHelpContext);
            }
            return symbolInspector;
        }
    }
}
