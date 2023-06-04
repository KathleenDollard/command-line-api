//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace System.CommandLine.Help
//{
//    public class SymbolData
//    {
//        public SymbolData(string? name, string? description, string parentCommandName, IEnumerable<string> completions)
//        {
//            Name = name ?? string.Empty;
//            Description = description ?? string.Empty;
//            ParentCommandName = parentCommandName;
//            Completions = completions;
//        }

//        public string? Name { get; }
//        public string Description { get; }

//         public string ParentCommandName { get; }
//        public IEnumerable<string> Completions { get; }
//    }

//    public class OptionArgData : SymbolData
//    {
//        public OptionArgData(string? name,
//                             string? description,
//                             string parentCommandName,
//                             IEnumerable<string> completions,
//                             Type valueType,
//                             object? defaultValue)
//            : base(name, description, parentCommandName, completions)
//        {
//            ValueType = valueType;
//            DefaultValue = defaultValue;
//        }

//        public Type ValueType { get; }
//        public object? DefaultValue { get; }

//    }
//}
