using System.Collections.Generic;

namespace System.CommandLine.Pipeline
{





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
