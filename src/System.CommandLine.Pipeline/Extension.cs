using System.Collections.Generic;

namespace System.CommandLine.Pipeline
{
    // Notes on variations from "System.CommandLine Extensibility"
    //
    // * We have history that the order of extension execution (and possibily initialization and teardown) is difficult and essential. We need to control this in almost all cases
    //    * Where the user needs something else to control ordering, allow inheritance of the Runner
    //    * This is addressed here with Category to get things rolling and to isolate the issue, but needs more attention
    //
    // * Extension and PipelineExtension are separated so users in normal settings can ignore the pipeline details
    //   * An alternative would be to simplify it enough to recombine, but details like whether something is activated (such as the user entering "-h" being a trigger being isolated to the help subsystem) may be surprising
    //
    // * I think we need one high level concept that knows configuration, CLI declaration. I find this most natural to call a parser
    //   (thus the user in building up their CLI builds a custom parser in concept), a CLI configuration that contains a declaration
    //   the second most natural and a RootCommand with a Config hanging off it the least natural. I used configuration as the most 
    //   clear in our current state
    //
 



    public class Extension
    {
        private readonly Dictionary<string, object> annotations = [];

        protected Extension(string name, PipelineExtension pipelineExtension)
        {
            Name = name;
            PipelineExtension  = pipelineExtension;
        }

        /// <summary>
        /// The name of the extension. 
        /// </summary>
        public string Name { get; }

        public PipelineExtension PipelineExtension { get; } 

        public void SetAnnotation(CliConfiguration configuration, string id, object value)
            => annotations.Add(id, value);

        public T? GetAnnotation<T>(ParseResult result, CliConfiguration configuration, string id)
            => annotations.TryGetValue(id, out var value) ? (T)value : default;

    }
}
