namespace System.CommandLine
{
    /// <summary>
    /// This is part of an effort to avoid Configuration dependencies on specific action
    /// types like help of completion and to help multiple extensions behave properly
    /// when used together. It both provides a home for action specific configuration and 
    /// a mechanism to include a feature with a single line on the CLI author's part (add the
    /// ConfigurableAction to the Configuration. They are held in CliConfiguration as a dictionary
    /// and having the key here simplifies adding to the dictionary.
    /// <br/>
    /// A ConfigurableAction (name is for debate) adds a symbol of any type and allows
    /// configuration. These present a surface area to the CLI author that is not distracted by
    /// or dependent on the specific symbol and allows multiple extensions to affect configuration. <br/>
    /// During CLI creation, at the start of Parse, the AddSymbol method of all of the ConfigurableActions on
    /// the CliConfiguration are are called, passing the RootCommand 
    /// <br/>
    /// The ConfigurableAction is expected to be the Action for the Symbol to make its configuration
    /// info available during parsing and invocation. This should not present a circular dependency
    /// problem because [Symbol].Action is a general type.
    /// <br/>
    /// The problem this solves is that the CliConfiguration needs an open set of configurations
    /// that do not reside in the System.CommandLine assembly and must be available to the action
    /// and may be needed during parsing. There are other solutions, for example keys and a dictionary,
    /// but this seemed less ugly.
    /// <br/>
    /// Note: There is currently no action because it is expected that the added symbol will contain this.
    /// Scenarios that do not involve a symbol (CLI author action) are not supported because we do not
    /// know how the action could be instigated.
    /// </summary>
    public abstract class CliSpecificConfiguration
    {
        
       protected CliSpecificConfiguration(string key)
        {
            Key = key;
        }

        public abstract void AddSymbol(CliCommand rootCommand);
        public string Key { get; }
    }
}
