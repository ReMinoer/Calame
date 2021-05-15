using System;
using Calame.Commands.Base;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Calame.Commands
{
    [CommandDefinition]
    public class RunDocumentCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Run";
        public override object IconKey => CalameIconKey.Play;

        [CommandHandler]
        public class CommandHandler : DocumentCommandHandlerBase<IRunnableDocument, RunDocumentCommand>
        {
            private Type _runCommandDefinitionType;
            private CommandDefinitionBase _runCommandDefinition;
            private CommandHandlerWrapper _commandHandler;
            private Command _command;

            private ICommandService _commandService;
            private ICommandRouter _commandRouter;

            protected override void RefreshContext(Command command, IRunnableDocument document)
            {
                base.RefreshContext(command, document);

                _command = command;

                if (_runCommandDefinitionType != Document?.RunCommandDefinitionType)
                {
                    _runCommandDefinitionType = Document?.RunCommandDefinitionType;
                    if (_runCommandDefinitionType != null)
                    {
                        if (_commandService == null)
                            _commandService = IoC.Get<ICommandService>();
                        if (_commandRouter == null)
                            _commandRouter = IoC.Get<ICommandRouter>();

                        _runCommandDefinition = _commandService.GetCommandDefinition(_runCommandDefinitionType);
                        _commandHandler = _commandRouter.GetCommandHandler(_runCommandDefinition);
                    }
                    else
                    {
                        _runCommandDefinition = null;
                        _commandHandler = null;
                    }
                }

                CommandDefinitionBase activeCommandDefinition = _runCommandDefinition ?? command.CommandDefinition;
                command.Text = activeCommandDefinition.Text;
                command.ToolTip = activeCommandDefinition.ToolTip;
                command.IconSource = activeCommandDefinition.IconSource;

                if (_commandHandler == null)
                {
                    command.Checked = false;
                    command.Visible = true;
                    return;
                }

                _commandHandler?.Update(command);
            }

            protected override bool CanRun(IRunnableDocument document)
            {
                return base.CanRun(document)
                    && _commandHandler != null
                    && _command.Enabled;
            }

            protected override void Run(IRunnableDocument document)
            {
                _commandHandler.Run(_command).Wait();
            }
        }
    }
}