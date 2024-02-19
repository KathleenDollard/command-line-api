using System.CommandLine.Extensions;

namespace System.CommandLine.RunnerExtension
{
    public class Runner : Extension
    {
        private bool _isActivated;

        public Runner()
            : base("Runner", CategoryRunner)
        {
            _isActivated = true;
        }

        public void DisableRunner() => _isActivated = false;

        public override bool GetIsActivated(ParseResult result)
        {
            return _isActivated;
        }

        public override bool Execute(ParseResult result)
        {
            if (!_isActivated)
            {
                return true; // While we have not done anything, it seems unlikely something else should run
            }

            // TODO: Review final design to see if this could have nulls
            var extensions = result.Configuration.Extensions
                .OrderBy(x => x.Category)
                .ToList();

            foreach (var extension in extensions)
            {
                if (extension.GetIsActivated(result))
                {
                    if (extension.Execute(result))
                    {  return true; }
                }
            }
            return true;
        }
    }
}
