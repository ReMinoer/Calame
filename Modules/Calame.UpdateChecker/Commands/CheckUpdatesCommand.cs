using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Calame.AutoUpdate;
using Calame.Commands.Base;
using Calame.Icons;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Microsoft.Extensions.Logging;

namespace Calame.UpdateChecker.Commands
{
    [CommandDefinition]
    public class CheckUpdatesCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Check _Updates...";
        public override object IconKey => CalameIconKey.CheckUpdates;

        [CommandHandler]
        public class CommandHandler : CommandHandlerBase<CheckUpdatesCommand>
        {
            private readonly IShell _shell;
            private readonly ILogger _logger;

            [Import]
            public IAutoUpdateConfiguration AutoUpdateConfiguration { get; set; }

            [ImportingConstructor]
            public CommandHandler(IShell shell, ILoggerProvider loggerProvider)
            {
                _shell = shell;
                _logger = loggerProvider.CreateLogger(nameof(UpdateChecker));
            }

            public override Task Run(Command command) => CheckUpdatesAndApply(AutoUpdateConfiguration, _shell, _logger);
        }

        static public async Task CheckUpdatesAndApply(IAutoUpdateConfiguration autoUpdateConfiguration, IShell shell, ILogger logger, bool silentIfUpToDate = false)
        {
            string installerFilePath = await GitHubAutoUpdater.CheckUpdatesAndAskUserToDownload(autoUpdateConfiguration, logger, silentIfUpToDate);
            if (installerFilePath is null)
                return;

            Process.Start("msiexec", $"/i {installerFilePath}");
            shell.Close();
        }
    }
}