using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame.LogConsole.ViewModels
{
    [Export(typeof(LogConsoleViewModel))]
    public sealed class LogConsoleViewModel : Tool
    {
        public override PaneLocation PreferredLocation => PaneLocation.Bottom;

        [ImportingConstructor]
        public LogConsoleViewModel(IShell shell)
        {
            DisplayName = "Log Console";
        }
    }
}