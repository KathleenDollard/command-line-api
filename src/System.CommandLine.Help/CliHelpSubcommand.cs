using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    public class CliHelpSubcommands : CliSection<InspectorCommandData>
    {

        public CliHelpSubcommands()
            : base(LocalizationResources.HelpCommandsTitle())
        { }

        public override IEnumerable<InspectorCommandData> GetData(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext)
            {
                return Enumerable.Empty<InspectorCommandData>();
            }

            var symbolInspector = CliHelpUtilities.SymbolInspector(helpContext);
            return symbolInspector.GetSubcommandData(helpContext.Command);
        }

        public override IEnumerable<CliOutputUnit>? GetBody(CliOutputContext outputContext)
        {
            if (outputContext is not HelpContext helpContext) { return null; }

            var unit = GetBodyTable(helpContext);
            return unit is null || !unit.Data.Any()
                ? null
                : new CliOutputUnit[] { unit };
        }

        private CliTable<InspectorCommandData>? GetBodyTable(HelpContext? helpContext)
        {
            if (helpContext?.Command is not CliCommand command)
            {
                return null;
            }

            var data = GetData(helpContext);

            var table = new CliTable<InspectorCommandData>(2, data)
            {
                IndentLevel = 1
            };
            table.Body[0] = data => GetFirstColumn(data);
            table.Body[1] = data => GetSecondColumn(data);
            return table;
        }

        private string GetFirstColumn(InspectorCommandData data)
            => GetUsage(data);

        private string GetSecondColumn(InspectorCommandData data)
        {
            var defaultValueDescription = GetDefaultValueText(data);
            return string.IsNullOrEmpty(defaultValueDescription)
            ? $"{data.Description}".Trim()
                : $"{data.Description} [{defaultValueDescription}]".Trim();

            //var symbolDescription = symbolInspector.GetDescription(command) ?? string.Empty;

            //var defaultValueDescription = GetCommandDefaultArgValues(command, symbolInspector);
            //return $"{symbolDescription} {defaultValueDescription}".Trim();
        }

        //private string GetCommandDefaultArgValues(CliCommand command, CliSymbolInspector symbolInspector)
        //{
        //    var args = command.Arguments
        //        .Where(arg => !arg.Hidden && arg.HasDefaultValue);

        //    return args.Count() switch
        //    {
        //        0 => "",
        //        1 => $"[{Default.DefaultValueTextAndLabel(args.First(), false)}]",
        //        _ => $"[{string.Join(", ", args.Select(arg => symbolInspector.GetDefaultValueText(arg, true)))}]"
        //    };
        //}

        protected virtual string GetUsage(InspectorCommandData data, bool showUsageOnBool = false, bool skipNameDefault = false)
            => CliHelpUtilities.GetUsage(data, showUsageOnBool, skipNameDefault);

        protected virtual string GetDefaultValueText(InspectorCommandData data, bool displaySymbolName = false)
            => CliHelpUtilities.GetDefaultValueText(data, displaySymbolName);


    }
}
