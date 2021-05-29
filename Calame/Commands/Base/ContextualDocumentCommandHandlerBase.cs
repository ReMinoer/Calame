using System;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;

namespace Calame.Commands.Base
{
    public abstract class ContextualDocumentCommandHandlerBase<TDocument, TDefinition> : DocumentCommandHandlerBase<TDocument, TDefinition>
        where TDocument : class, IDocument
        where TDefinition : CommandDefinition
    {
        private Type _commandDefinitionType;
        private CommandDefinitionBase _commandDefinition;
        private CommandHandlerWrapper _commandHandler;
        private Command _command;

        private ICommandService _commandService;
        private ICommandRouter _commandRouter;

        protected abstract bool VisibleEvenIfNotUsed { get; }
        protected abstract Type GetCommandDefinitionType(TDocument document);

        protected override void RefreshContext(Command command, TDocument document)
        {
            base.RefreshContext(command, document);

            _command = command;

            Type commandDefinitionType = document != null ? GetCommandDefinitionType(document) : null;
            if (_commandDefinitionType != commandDefinitionType)
            {
                _commandDefinitionType = commandDefinitionType;

                if (_commandDefinitionType != null)
                {
                    if (_commandService == null)
                        _commandService = IoC.Get<ICommandService>();
                    if (_commandRouter == null)
                        _commandRouter = IoC.Get<ICommandRouter>();

                    _commandDefinition = _commandService.GetCommandDefinition(_commandDefinitionType);
                    _commandHandler = _commandRouter.GetCommandHandler(_commandDefinition);
                }
                else
                {
                    _commandDefinition = null;
                    _commandHandler = null;
                }
            }

            CommandDefinitionBase activeCommandDefinition = _commandDefinition ?? command.CommandDefinition;
            command.Text = activeCommandDefinition.Text;
            command.ToolTip = activeCommandDefinition.ToolTip;
            command.IconSource = activeCommandDefinition.IconSource;

            if (_commandHandler == null)
            {
                command.Checked = false;
                command.Visible = VisibleEvenIfNotUsed;
                return;
            }

            command.Checked = false;
            command.Visible = true;
            _commandHandler.Update(command);
        }

        protected override bool CanRun(TDocument document)
        {
            return base.CanRun(document)
                && _commandHandler != null
                && _command.Enabled;
        }

        protected override void Run(TDocument document)
        {
            _commandHandler.Run(_command).Wait();
        }
    }
}