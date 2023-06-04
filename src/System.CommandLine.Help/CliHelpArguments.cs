using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpArguments : CliSection<InspectorArgumentData>
    {

        public CliHelpArguments()
           : base(LocalizationResources.HelpArgumentsTitle())
        { }

        public override IEnumerable<InspectorArgumentData> GetData(CliOutputContext outputContext)
        {  
            if (outputContext is not HelpContext helpContext)
            {
                return Enumerable.Empty<InspectorArgumentData>();
            }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            return symbolInspector.GetArgumentData(helpContext.Command);
        }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext) { return null; }

            var unit = GetBodyTable(helpContext);
            return unit is null || !unit.Data.Any()
                ? null
                : new CliOutputUnit[] { unit };
        }

        private CliTable<InspectorArgumentData>? GetBodyTable(HelpContext? helpContext)
        {
            if (helpContext?.Command is not CliCommand command)
            {
                return null;
            }

            var data = GetData(helpContext);

            var table = new CliTable<InspectorArgumentData>(2, data)
            {
                IndentLevel = 1
            };
            table.Body[0] = dataItem => GetFirstColumn(dataItem);
            table.Body[1] = dataItem => GetSecondColumn(dataItem);
            return table;
        }

        protected virtual string GetFirstColumn(InspectorArgumentData data, bool showUsageOnBool = false, bool skipNameDefault = false)
            => GetUsage(data, showUsageOnBool, skipNameDefault);

        protected virtual string GetSecondColumn(InspectorArgumentData data)
        {
            var defaultValueDescription = GetDefaultValueText(data);
            return string.IsNullOrEmpty(defaultValueDescription)
                ? $"{data.Description}".Trim()
                : $"{data.Description} [{defaultValueDescription}]".Trim();
        }

        protected virtual string GetUsage(InspectorArgumentData data, bool showUsageOnBool = false, bool skipNameDefault = false)
            => CliHelpUtilities.GetUsage(data, showUsageOnBool, skipNameDefault);

        protected virtual string GetDefaultValueText(InspectorArgumentData data, bool displaySymbolName = false)
            => CliHelpUtilities.GetDefaultValueText(data, displaySymbolName);

    }
}
