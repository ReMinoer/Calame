using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calame.Commands;
using Calame.ContentFileTypes;
using Calame.Icons;
using Calame.PropertyGrid.Controls;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Engine;
using Glyph.Pipeline;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : CalameTool<IDocumentContext>, IHandle<ISelectionSpread<object>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public IIconProvider IconProvider { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }

        private object _selectedObject;
        public object SelectedObject
        {
            get => _selectedObject;
            set => SetValue(ref _selectedObject, value);
        }

        private string _workingDirectory;
        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => SetValue(ref _workingDirectory, value);
        }

        private IRawContentLibrary _rawContentLibrary;
        public IRawContentLibrary RawContentLibrary
        {
            get => _rawContentLibrary;
            set => SetValue(ref _rawContentLibrary, value);
        }

        public IContentFileTypeResolver ContentFileTypeResolver { get; }
        public IList<Type> NewItemTypeRegistry { get; }

        public TargetableCommand PreviousCommand { get; }
        public TargetableCommand NextCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand OpenFolderCommand { get; }
        public AsyncCommand DirtyDocumentCommand { get; }
        public RelayCommand SelectItemCommand { get; }

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider, ICommandService commandService,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEditorProvider[] editorProviders, IContentFileTypeResolver contentFileTypeResolver)
            : base(shell, eventAggregator)
        {
            DisplayName = "Property Grid";

            ContentFileTypeResolver = contentFileTypeResolver;
            NewItemTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToList();

            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;

            PreviousCommand = commandService.GetTargetableCommand<PreviousSelectionCommand>();
            NextCommand = commandService.GetTargetableCommand<NextSelectionCommand>();

            OpenFolderCommand = new RelayCommand(
                path => Process.Start((string)path),
                path => !string.IsNullOrWhiteSpace((string)path));
            OpenFileCommand = new RelayCommand(
                async path => await shell.OpenFileAsync((string)path, editorProviders, WorkingDirectory),
                path => !string.IsNullOrWhiteSpace((string)path));

            DirtyDocumentCommand = new AsyncCommand(() => EventAggregator.PublishAsync(new DirtyMessage(CurrentDocument, SelectedObject)));
            SelectItemCommand = new RelayCommand(x =>
                {
                    ISelectionRequest<object> selectionRequest;
                    switch (((ItemEventArgs)x).Item)
                    {
                        case IGlyphData data:
                            selectionRequest = new SelectionRequest<IGlyphData>(CurrentDocument, data);
                            break;
                        case IGlyphComponent component:
                            selectionRequest = new SelectionRequest<IGlyphComponent>(CurrentDocument, component);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    EventAggregator.PublishAsync(selectionRequest).Wait();
                }
            );
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument)
        {
            SelectedObject = null;
            WorkingDirectory = activeDocument.WorkingDirectory;
            RawContentLibrary = (activeDocument as IDocumentContext<GlyphEngine>)?.Context.ContentLibrary as IRawContentLibrary;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            SelectedObject = null;
            WorkingDirectory = null;
            RawContentLibrary = null;

            return Task.CompletedTask;
        }

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            SelectedObject = message.Item;
            return Task.CompletedTask;
        }
    }
}