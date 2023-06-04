using System.Collections.Generic;
using System.CommandLine.CliOutput;
using System.Linq;

namespace System.CommandLine.Help
{
    // KAD: This class only exists because of current intuition that it may be needed. Delete if not used for meaningful stuff when finalizing design.
    public class CliHelpBuilder : CliOutputRenderer, IHelpBuilder
    {
        // This temporarily satisfies IHelpBuilder
        public virtual void Write(HelpContext helpContext)
        {
            _ = helpContext ?? throw new ArgumentNullException(nameof(helpContext));

             base.Write(helpContext);
        }

    }
}
