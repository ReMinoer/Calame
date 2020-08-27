using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Calame.PropertyGrid.Controls;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : CalameTool<IDocumentContext>, IHandle<ISelectionSpread<object>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        private SelectionHistory _selectionHistory;
        public SelectionHistoryManager SelectionHistoryManager { get; }

        public IIconProvider IconProvider { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }

        private object _selectedObject;
        public object SelectedObject
        {
            get => _selectedObject;
            set => SetValue(ref _selectedObject, value);
        }

        public IList<Type> NewItemTypeRegistry { get; }

        public AsyncCommand PreviousCommand { get; }
        public AsyncCommand NextCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand OpenFolderCommand { get; }
        public AsyncCommand DirtyDocumentCommand { get; }
        public RelayCommand SelectItemCommand { get; }

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider, SelectionHistoryManager selectionHistoryManager,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEditorProvider[] editorProviders)
            : base(shell, eventAggregator)
        {
            DisplayName = "Property Grid";

            NewItemTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToList();

            SelectionHistoryManager = selectionHistoryManager;
            SelectionHistoryManager.CurrentDocumentHistoryChanged += OnCurrentDocumentHistoryChanged;

            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;

            PreviousCommand = new AsyncCommand(
                () => _selectionHistory?.SelectPreviousAsync(),
                () => _selectionHistory?.HasPrevious ?? false);
            NextCommand = new AsyncCommand(
                () => _selectionHistory?.SelectNextAsync(),
                () => _selectionHistory?.HasNext ?? false);

            OpenFolderCommand = new RelayCommand(path => Process.Start((string)path));
            OpenFileCommand = new RelayCommand(async path => await shell.OpenFileAsync((string)path, editorProviders));

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
            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            SelectedObject = null;
            return Task.CompletedTask;
        }

        private void OnCurrentDocumentHistoryChanged(object sender, EventArgs e)
        {
            if (_selectionHistory != null)
            {
                _selectionHistory.HasNextChanged -= OnHasNextSelectionChanged;
                _selectionHistory.HasPreviousChanged -= OnHasPreviousSelectionChanged;
            }

            _selectionHistory = SelectionHistoryManager.CurrentDocumentHistory;

            PreviousCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();

            if (_selectionHistory != null)
            {
                _selectionHistory.HasPreviousChanged += OnHasPreviousSelectionChanged;
                _selectionHistory.HasNextChanged += OnHasNextSelectionChanged;
            }
        }

        private void OnHasPreviousSelectionChanged(object sender, EventArgs e) => PreviousCommand.RaiseCanExecuteChanged();
        private void OnHasNextSelectionChanged(object sender, EventArgs e) => NextCommand.RaiseCanExecuteChanged();

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            SelectedObject = message.Item;
            return Task.CompletedTask;
        }
    }
}