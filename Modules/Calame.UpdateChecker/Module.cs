using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.AutoUpdate;
using Calame.UpdateChecker.Commands;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu;
using Microsoft.Extensions.Logging;

namespace Calame.UpdateChecker
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        private readonly IShell _shell;
        private readonly ILoggerProvider _loggerProvider;

        [Import]
        public IAutoUpdateConfiguration AutoUpdateConfiguration { get; set; }

        [ImportingConstructor]
        public Module(IShell shell, ILoggerProvider loggerProvider)
        {
            _shell = shell;
            _loggerProvider = loggerProvider;
        }

        public override async Task PostInitializeAsync()
        {
            await base.PostInitializeAsync();
            if (CalameUtils.IsDevelopmentBuild())
                return;

            await CheckUpdatesCommand.CheckUpdatesAndApply(AutoUpdateConfiguration, _shell, _loggerProvider.CreateLogger(nameof(UpdateChecker)), silentIfUpToDate: true);
        }
        
        [Export]
        static public readonly MenuItemDefinition CheckEditorUpdate = new CommandMenuItemDefinition<CheckUpdatesCommand>(CalameMenus.FileSystemGroup, 10);
    }
}