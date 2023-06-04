using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpSynopsis : CliSection<InspectorCommandData>
    {
        public CliHelpSynopsis()
            : base(LocalizationResources.HelpDescriptionTitle(), true)
        { }


        public override IEnumerable<InspectorCommandData> GetData(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext)
            {
                return Enumerable.Empty<InspectorCommandData>();
            }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            return new InspectorCommandData[]
                {
                    symbolInspector.GetCommandData(helpContext.Command, null)
                };
        }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            var data = GetData(outputContext);
            return data.Any()
                ? new CliOutputUnit[] { new CliText(data.First().Description, 1) }
                : null;
        }
    }
}
