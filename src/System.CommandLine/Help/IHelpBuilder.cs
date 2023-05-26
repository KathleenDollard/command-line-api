using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine.Help
{
    public interface IHelpBuilder
    {
        void Write(HelpContext context);

    }
}
