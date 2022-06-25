using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Commands;
using Calame.ContentFileTypes;
using Calame.DocumentContexts;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Glyph.Tools.UndoRedo;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : CalameTool<IDocumentContext>, IHandle<ISelectionSpread<object>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        private readonly IEditorProvider[] _editorProviders;
        public IIconProvider IconProvider { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }

        private object _selectedObject;
        public object SelectedObject
        {
            get => _selectedObject;
            set => SetValue(ref _selectedObject, value);
        }

        private IRawContentLibraryContext _rawContentLibraryContext;
        public IRawContentLibraryContext RawContentLibraryContext
        {
            get => _rawContentLibraryContext;
            private set => SetValue(ref _rawContentLibraryContext, value);
        }

        private ICommand _selectItemCommand;
        public ICommand SelectItemCommand
        {
            get => _selectItemCommand;
            private set => SetValue(ref _selectItemCommand, value);
        }

        private IUndoRedoStack _undoRedoStack;
        public IUndoRedoStack UndoRedoStack
        {
            get => _undoRedoStack;
            private set => SetValue(ref _undoRedoStack, value);
        }
        
        public IContentFileTypeResolver ContentFileTypeResolver { get; }
        public IList<Type> NewItemTypeRegistry { get; }

        public TargetableCommand PreviousCommand { get; }
        public TargetableCommand NextCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand OpenFolderCommand { get; }

        protected override object IconKey => CalameIconKey.PropertyGrid;

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider, ICommandService commandService,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEditorProvider[] editorProviders, IContentFileTypeResolver contentFileTypeResolver)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Property Grid";

            ContentFileTypeResolver = contentFileTypeResolver;
            NewItemTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToList();

            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;

            PreviousCommand = commandService.GetTargetableCommand<PreviousSelectionCommand>();
            NextCommand = commandService.GetTargetableCommand<NextSelectionCommand>();

            _editorProviders = editorProviders;
            OpenFolderCommand = new RelayCommand(OnOpenFolder, CanOpenFolder);
            OpenFileCommand = new RelayCommand(OnOpenFile, CanOpenFile);
        }

        private bool CanOpenFolder(object path) => !string.IsNullOrWhiteSpace((string)path);
        private void OnOpenFolder(object path)
        {
            Process.Start((string)path);
        }

        private bool CanOpenFile(object path) => !string.IsNullOrWhiteSpace((string)path);
        private async void OnOpenFile(object path)
        {
            await Shell.OpenFileAsync((string)path, _editorProviders, RawContentLibraryContext?.RawContentLibrary?.WorkingDirectory);
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument)
        {
            SelectedObject = null;

            RawContentLibraryContext = activeDocument.TryGetContext<IRawContentLibraryContext>();
            SelectItemCommand = activeDocument.TryGetContext<ISelectionContext>()?.GetSelectionCommand();
            UndoRedoStack = activeDocument.TryGetContext<IUndoRedoContext>()?.UndoRedoStack;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            SelectedObject = null;

            SelectItemCommand = null;
            RawContentLibraryContext = null;

            return Task.CompletedTask;
        }

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            SelectedObject = message.Item;
            return Task.CompletedTask;
        }
    }
}