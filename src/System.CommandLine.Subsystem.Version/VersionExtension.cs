using System.CommandLine.Extended.Annotations;

namespace System.CommandLine.Subsystem
{
    public class VersionSubsystem(IAnnotationProvider? annotationProvider = null) : CliSubsystem(annotationProvider, "Version")
    {
        // TODO: Should we block adding version anywhere but root?
        public void SetVersion(CliSymbol symbol, string description) 
            => SetAnnotation(symbol, VersionAnnotations.Description, description);

        public AnnotationAccessor<string> Version => new(this, VersionAnnotations.Version);
    }
}
