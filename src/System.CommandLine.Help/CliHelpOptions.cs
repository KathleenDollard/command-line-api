using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpOptions : CliSection<InspectorOptionData>
    {

        public CliHelpOptions()
            : base(LocalizationResources.HelpOptionsTitle())
        { }

        public override IEnumerable<InspectorOptionData> GetData(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext)
            {
                return Enumerable.Empty<InspectorOptionData>();
            }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            return symbolInspector.GetOptionData(helpContext.Command);
        }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext) { return null; }

            var unit = GetBodyTable(helpContext);
            return unit is null || !unit.Data.Any()
                ? null
                : new CliOutputUnit[] { unit };
        }

        private CliTable<InspectorOptionData>? GetBodyTable(HelpContext helpContext)
        {
            if (helpContext?.Command is not CliCommand command)
            {
                return null;
            }

            var data = GetData(helpContext);

            var table = new CliTable<InspectorOptionData>(2, data)
            {
                IndentLevel = 1
            };
            table.Body[0] = dataItem => GetFirstColumn(dataItem);
            table.Body[1] = dataItem => GetSecondColumn(dataItem);
            return table;

        }

        private string GetFirstColumn(InspectorOptionData data, bool showUsageOnBool = false, bool skipNameDefault = false)
            => GetUsage(data, showUsageOnBool,skipNameDefault);

        private string GetSecondColumn(InspectorOptionData data)
        {
            var defaultValueDescription = GetDefaultValueText(data);

            return string.IsNullOrEmpty(defaultValueDescription)
                ? $"{data.Description}".Trim()
                : $"{data.Description} [{defaultValueDescription}]".Trim();
        }

        protected virtual string GetUsage(InspectorOptionData data, bool showUsageOnBool = false, bool skipNameDefault = false)
            => CliHelpUtilities.GetUsage(data, showUsageOnBool, skipNameDefault);

        protected virtual string GetDefaultValueText(InspectorOptionData data, bool displaySymbolName = false)
            => CliHelpUtilities.GetDefaultValueText(data, displaySymbolName);
    }
}
