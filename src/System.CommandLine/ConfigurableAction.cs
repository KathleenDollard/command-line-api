using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine
{
    /// <summary>
    /// This is part of an effort to avoid Configuration dependencies on specific action
    /// types like help of completion.<br/>
    /// A ConfigurableAction (name is for debate) contains a symbol of any type and allows
    /// configuration. These present a surface area to the CLI author that is not distracted by
    /// or dependent on the specific symbol. <br/>
    /// During CLI creation, probably at the start of Parse, all of the ConfigurableActions on
    /// the CliConfiguration are added to the Root. <br/>
    /// The symbol for the CLI action must have a constructor that takes the ConfigurableAction
    /// making it available during parsing and invocation. <br/>
    /// The problem this solves is that the CliConfiguration needs an open set of configurations
    /// that do not reside in the System.CommandLine assembly and must be available to the action
    /// and may be needed during parsing. There are other solutions, for example keys and a dictionary,
    /// but this seemed less ugly.
    /// </summary>
    public class ConfigurableAction
    {


        public ConfigurableAction(CliSymbol symbol, string key)
        {
            Symbol = symbol;
            Key = key;
        }

        public CliSymbol Symbol { get; }
        public string Key { get; }
    }
}
