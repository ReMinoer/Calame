using System;
using Calame.DataModelViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.DataModelViewer.Commands.Base
{
    public abstract class EditorCommandHandlerBase<TEditor, TCommandDefinition> : DataModelViewerCommandHandlerBase<TCommandDefinition>
        where TEditor : class, IEditor
        where TCommandDefinition : CommandDefinition
    {
        private TEditor _editor;

        protected override void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            base.OnActiveDocumentChanged(sender, e);
            _editor = Document?.Editor as TEditor;
        }

        protected override sealed void RefreshContext(Command command, DataModelViewerViewModel document)
        {
            base.RefreshContext(command, document);

            if (_editor == null && Document != null)
                _editor = Document.Editor as TEditor;

            RefreshContext(command, _editor);
        }

        protected override sealed bool CanRun(DataModelViewerViewModel document)
        {
            return base.CanRun(document)
                && _editor != null
                && CanRun(_editor);
        }

        protected override sealed void UpdateStatus(Command command, DataModelViewerViewModel document)
        {
            base.UpdateStatus(command, document);
            UpdateStatus(command, _editor);
        }

        protected override sealed void Run(DataModelViewerViewModel document)
        {
            Run(_editor);
        }

        protected virtual void RefreshContext(Command command, TEditor editor) {}
        protected virtual bool CanRun(TEditor editor) => true;
        protected virtual void UpdateStatus(Command command, TEditor editor) { }
        protected abstract void Run(TEditor editor);
    }
}