using System;
using System.Collections;
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
using Calame.Utils;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Tools.UndoRedo;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : CalameTool<IDocumentContext>, IHandle<ISelectionSpread<object>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        private readonly IEditorProvider[] _editorProviders;
        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }

        private object _selectedObject;
        public object SelectedObject
        {
            get => _selectedObject;
            private set => SetValue(ref _selectedObject, value);
        }

        private IComposable _selectedComposable;
        public IComposable SelectedComposable
        {
            get => _selectedComposable;
            private set => SetValue(ref _selectedComposable, value);
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
        private ICommand _addComponentCommand;

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

        public ICommand AddComponentCommand
        {
            get => _addComponentCommand;
            set => Set(ref _addComponentCommand, value);
        }

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
            IconDescriptor = IconDescriptorManager.GetDescriptor();

            PreviousCommand = commandService.GetTargetableCommand<PreviousSelectionCommand>();
            NextCommand = commandService.GetTargetableCommand<NextSelectionCommand>();

            _editorProviders = editorProviders;
            OpenFolderCommand = new RelayCommand(OnOpenFolder, CanOpenFolder);
            OpenFileCommand = new RelayCommand(OnOpenFile, CanOpenFile);
        }

        private bool CanOpenFolder(object path) => !string.IsNullOrWhiteSpace((string)path);
        private void OnOpenFolder(object path)
        {
            Process.Start(new ProcessStartInfo((string)path) { UseShellExecute = true });
        }

        private bool CanOpenFile(object path) => !string.IsNullOrWhiteSpace((string)path);
        private async void OnOpenFile(object path)
        {
            await Shell.OpenFileAsync((string)path, _editorProviders, RawContentLibraryContext?.RawContentLibrary?.WorkingDirectory);
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument)
        {
            SelectedObject = null;
            SelectedComposable = null;

            RawContentLibraryContext = activeDocument.TryGetContext<IRawContentLibraryContext>();
            SelectItemCommand = activeDocument.TryGetContext<ISelectionContext>()?.GetSelectionCommand();
            UndoRedoStack = activeDocument.TryGetContext<IUndoRedoContext>()?.UndoRedoStack;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            SelectedObject = null;
            SelectedComposable = null;

            SelectItemCommand = null;
            RawContentLibraryContext = null;

            return Task.CompletedTask;
        }

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            SelectedObject = message.Item;
            SelectedComposable = SelectedObject as IComposable;
            
            if (SelectedComposable != null)
                AddComponentCommand = new AddCollectionItemCommand(SelectedComposable.Composition, InsertItem, NewItemTypeRegistry, IconProvider, IconDescriptor);
            else
                AddComponentCommand = null;

            return Task.CompletedTask;
        }

        private void InsertItem(int index, object item)
        {
            IList list = SelectedComposable.Composition;

            UndoRedoStack.Execute($"Add {item} to {SelectedComposable}.",
                () =>
                {
                    (item as IRestorable)?.Restore();
                    list.Insert(index, item);
                },
                () =>
                {
                    list.RemoveAt(index);
                    (item as IRestorable)?.Store();
                },
                null,
                () => (item as IDisposable)?.Dispose());
        }
    }
}