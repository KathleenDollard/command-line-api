using System.CommandLine.Pipeline;

namespace System.CommandLine.VersionExtension
{
    // TODO: Remove the duplication in System.CommandLine.VersionExtension.VersionExtension
    public class VersionExtension : Extension
    {

        public VersionExtension() : base("Version", new VersionPipelineExtension())
        { }

        public new VersionPipelineExtension PipelineExtension { get; }

        public void SetVersion(CliConfiguration configuration, string version)
            => SetAnnotation(configuration, VersionAnnotations.Version, version);

        public string? GetDescription(ParseResult result, CliConfiguration configuration, string id)
          => GetAnnotation<string>(result, configuration, VersionAnnotations.Version);


    }
}
