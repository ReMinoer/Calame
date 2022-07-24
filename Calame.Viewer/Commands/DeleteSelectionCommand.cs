using System.Linq;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.Viewer.Commands.Base;
using Diese.Collections;
using Gemini.Framework.Commands;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.Viewer.Commands
{
    [CommandDefinition]
    public class DeleteSelectionCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Delete Selection";
        public override object IconKey => CalameIconKey.Delete;

        [CommandHandler]
        public class CommandHandler : ViewerDocumentCommandHandlerBase<IViewerDocument, DeleteSelectionCommand>
        {
            protected override bool CanRun(IViewerDocument document)
            {
                if (!base.CanRun(document))
                    return false;

                object selectedItem = document.Viewer.LastSelection?.Item;
                if (selectedItem is IGlyphComponent component && component.Parent != null && component.Parent.Opened)
                    return true;
                if (selectedItem is IGlyphData data && data.ParentSource != null)
                    return true;

                return false;
            }

            protected override void Run(IViewerDocument document)
            {
                switch (document.Viewer.LastSelection?.Item)
                {
                    case IGlyphComponent component:
                    {
                        IGlyphContainer parent = component.Parent;

                        document.UndoRedoManager.Execute($"Remove component {component.Name} from parent {parent}",
                            () =>
                            {
                                IGlyphContainer newSelection = Sequence.AggregateExclusive(component, x => x.Parent).FirstOrDefault(document.CanSelect);
                                document.SelectAsync(newSelection).Wait();

                                parent.Unlink(component);
                                component.Store();
                            },
                            () =>
                            {
                                component.Restore();
                                parent.Link(component);

                                IGlyphComponent newSelection = Sequence.Aggregate(component, x => x.Parent).FirstOrDefault(document.CanSelect);
                                document.SelectAsync(newSelection).Wait();
                            }
                        );
                        break;
                    }
                    case IGlyphData data:
                    {
                        IGlyphDataSource parentSource = data.ParentSource;
                        int index = parentSource.IndexOf(data);

                        document.UndoRedoManager.Execute($"Remove data {data} from parent source {parentSource}",
                            () =>
                            {
                                IGlyphData newSelection = Sequence.AggregateExclusive(data, x => x.ParentSource.Owner).FirstOrDefault(document.CanSelect);
                                document.SelectAsync(newSelection).Wait();

                                parentSource.Unset(index);
                                (data as IRestorable)?.Store();
                            },
                            () =>
                            {
                                (data as IRestorable)?.Restore();
                                parentSource.Set(index, data);

                                IGlyphData newSelection = Sequence.Aggregate(data, x => x.ParentSource.Owner).FirstOrDefault(document.CanSelect);
                                document.SelectAsync(newSelection).Wait();
                            }
                        );
                        break;
                    }
                }
            }
        }
    }
}