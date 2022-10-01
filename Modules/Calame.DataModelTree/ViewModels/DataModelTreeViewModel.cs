using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Tools.UndoRedo;

namespace Calame.DataModelTree.ViewModels
{
    [Export(typeof(DataModelTreeViewModel))]
    public sealed class DataModelTreeViewModel : CalameTool<IDocumentContext<IRootDataContext>>, IHandle<ISelectionSpread<IGlyphData>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }

        private readonly Type[] _newTypeRegistry;
        private ISelectionContext<IGlyphData> _selectionContext;
        private IUndoRedoContext _undoRedoContext;

        private readonly TreeViewItemModelBuilder<IGlyphData> _dataItemBuilder;
        private readonly TreeViewItemModelBuilder<IGlyphDataChildrenSource> _childrenSourceItemBuilder;
        
        private readonly ICommand _removeCommand;

        private IRootDataContext _rootDataContext;
        public IRootDataContext RootDataContext
        {
            get => _rootDataContext;
            private set => SetValue(ref _rootDataContext, value);
        }

        private object _selection;
        public object Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                _selectionContext.SelectAsync(_selection as IGlyphData).Wait();
            }
        }

        protected override object IconKey => CalameIconKey.DataModelTree;

        [ImportingConstructor]
        public DataModelTreeViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Data Model Tree";

            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor();
            IIconDescriptor<IGlyphData> dataIconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphData>();

            _newTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToArray();
            _removeCommand = new RelayCommand(OnRemove, CanRemove);

            _dataItemBuilder = new TreeViewItemModelBuilder<IGlyphData>()
                .DisplayName(x => x.DisplayName, nameof(IGlyphData.DisplayName))
                .ChildrenSource(x => new CompositeReadOnlyObservableList<object>
                (
                    new EnumerableReadOnlyObservableList<object>(x.Children),
                    new EnumerableReadOnlyObservableList<object>(x.ChildrenSources)
                ), x => ObservableHelpers.OnPropertyChanged(x as INotifyPropertyChanged, nameof(IGlyphData.Children), nameof(IGlyphData.ChildrenSources)))
                .IconDescription(x => dataIconDescriptor.GetIcon(x))
                .ContextMenuItems(GetContextMenuItems)
                .DraggedDataProvider(x => () => new DraggedData(DragDropEffects.Move, new DataObject(typeof(IGlyphData), x)))
                .DragEnterAction(x => e => OnDragOverData(e, x))
                .DragOverAction(x => e => OnDragOverData(e, x))
                .DropAction(x => e => OnDropOnData(e, x));

            _childrenSourceItemBuilder = new TreeViewItemModelBuilder<IGlyphDataChildrenSource>()
                .DisplayName(x => x.PropertyName)
                .FontWeight(_ => FontWeights.Bold)
                .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Children), nameof(IGlyphDataChildrenSource.Children))
                .IconDescription(x => IconDescriptor.GetIcon(x.Children), nameof(IGlyphDataChildrenSource.Children))
                .IsHeader(_ => true)
                .QuickCommand(x => CreateAddCommand(x), nameof(IGlyphDataChildrenSource.Children))
                .QuickCommandIconDescription(_ => new IconDescription(CalameIconKey.Add, Brushes.DarkGreen, margin: 0.5))
                .QuickCommandLabel(_ => "Add")
                .QuickCommandToolTip(_ => "Add")
                .DragEnterAction(x => e => OnDragOverChildrenSource(e, x))
                .DragOverAction(x => e => OnDragOverChildrenSource(e, x))
                .DropAction(x => e => OnDropOnChildrenSource(e, x));
        }

        private void OnDragOverData(DragEventArgs dragEventArgs, IGlyphData dropTarget)
        {
            dragEventArgs.Effects = IsValidDrop(dragEventArgs, dropTarget, out _, out _, out _)
                ? DragDropEffects.Move
                : DragDropEffects.None;

            dragEventArgs.Handled = true;
        }

        private void OnDragOverChildrenSource(DragEventArgs dragEventArgs, IGlyphDataChildrenSource dropTarget)
        {
            dragEventArgs.Effects = IsValidDrop(dragEventArgs, dropTarget, out _, out _, out _)
                ? DragDropEffects.Move
                : DragDropEffects.None;

            dragEventArgs.Handled = true;
        }

        private void OnDropOnData(DragEventArgs dragEventArgs, IGlyphData dropTarget)
        {
            if (dropTarget.ParentSource is IGlyphDataChildrenSource childrenSource
                && IsValidDrop(dragEventArgs, dropTarget, out IGlyphData draggedData, out int oldIndex, out int newIndex))
                Drop(draggedData, childrenSource, oldIndex, newIndex);
        }

        private void OnDropOnChildrenSource(DragEventArgs dragEventArgs, IGlyphDataChildrenSource dropTarget)
        {
            if (IsValidDrop(dragEventArgs, dropTarget, out IGlyphData draggedData, out int oldIndex, out int newIndex) && draggedData.ParentSource != dropTarget)
                Drop(draggedData, dropTarget, oldIndex, newIndex);
        }

        private void Drop(IGlyphData draggedData, IGlyphDataChildrenSource targetSource, int oldIndex, int newIndex)
        {
            if (draggedData.ParentSource == targetSource)
            {
                if (newIndex == targetSource.Count)
                    newIndex--;
                if (oldIndex == newIndex)
                    return;
            }

            IGlyphDataSource oldDataSource = draggedData.ParentSource;
            IGlyphDataSource newDataSource = targetSource;

            if (oldDataSource == newDataSource)
            {
                _undoRedoContext?.UndoRedoStack.Execute($"Move data {draggedData} to index {newIndex} in {newDataSource}.",
                    () => newDataSource.Move(oldIndex, newIndex),
                    () => newDataSource.Move(newIndex, oldIndex)
                );
            }
            else
            {
                _undoRedoContext?.UndoRedoStack.Execute($"Move data {draggedData} to index {newIndex} in {newDataSource}.",
                    () =>
                    {
                        oldDataSource.RemoveAt(oldIndex);
                        newDataSource.Insert(newIndex, draggedData);
                    },
                    () =>
                    {
                        newDataSource.RemoveAt(newIndex);
                        oldDataSource.Insert(oldIndex, draggedData);
                    }
                );
            }
        }

        private bool IsValidDrop(DragEventArgs dragEventArgs, IGlyphData dropTarget, out IGlyphData draggedData, out int oldIndex, out int newIndex)
        {
            draggedData = null;
            oldIndex = -1;
            newIndex = -1;

            if (dropTarget.ParentSource is null)
                return false;

            if (!dragEventArgs.Data.GetDataPresent(typeof(IGlyphData)))
                return false;

            draggedData = (IGlyphData)dragEventArgs.Data.GetData(typeof(IGlyphData));
            if (draggedData?.ParentSource is null)
                return false;

            oldIndex = draggedData.ParentSource.IndexOf(draggedData);
            newIndex = dropTarget.ParentSource.IndexOf(dropTarget);

            if (!draggedData.ParentSource.CanRemoveAt(oldIndex))
                return false;
            if (!dropTarget.ParentSource.CanInsert(newIndex, draggedData))
                return false;

            return true;
        }

        private bool IsValidDrop(DragEventArgs dragEventArgs, IGlyphDataChildrenSource dropTarget, out IGlyphData draggedData, out int oldIndex, out int newIndex)
        {
            draggedData = null;
            oldIndex = -1;
            newIndex = -1;

            if (!dragEventArgs.Data.GetDataPresent(typeof(IGlyphData)))
                return false;

            draggedData = (IGlyphData)dragEventArgs.Data.GetData(typeof(IGlyphData));
            if (draggedData?.ParentSource is null)
                return false;

            oldIndex = draggedData.ParentSource.IndexOf(draggedData);
            newIndex = dropTarget.Count;

            if (!draggedData.ParentSource.CanRemoveAt(oldIndex))
                return false;
            if (!dropTarget.CanInsert(newIndex, draggedData))
                return false;

            return true;
        }

        private AddCollectionItemCommand CreateAddCommand(IGlyphDataChildrenSource childrenSource, IGlyphData insertTarget = null, bool afterTarget = false)
        {
            if (!(childrenSource.Children is IList list))
                return null;

            return new AddCollectionItemCommand(list,
                (i, x) => AddDataChild(childrenSource, i, (IGlyphData)x), _newTypeRegistry, IconProvider, IconDescriptor, insertTarget, afterTarget);
        }

        private void AddDataChild(IGlyphDataChildrenSource childrenSource, int index, IGlyphData child)
        {
            _undoRedoContext?.UndoRedoStack.Execute($"Add data {child} to source {childrenSource}.",
                () =>
                {
                    (child as IRestorable)?.Restore();
                    childrenSource.Insert(index, child);
                    Selection = child;
                },
                () =>
                {
                    Selection = child.ParentSource.Owner;
                    childrenSource.RemoveAt(index);
                    (child as IRestorable)?.Store();
                },
                null,
                () => (child as IDisposable)?.Dispose());
        }

        private IEnumerable GetContextMenuItems(IGlyphData data)
        {
            if (data.ParentSource != null)
            {
                int dataIndex = data.ParentSource.IndexOf(data);
                bool canInsertAbove = data.ParentSource.CanInsert(dataIndex);
                bool canInsertBelow = data.ParentSource.CanInsert(dataIndex + 1);

                if (data.ParentSource is IGlyphDataChildrenSource childrenSource && (canInsertAbove || canInsertBelow))
                {
                    if (canInsertAbove)
                    {
                        var insertAboveItem = new MenuItem
                        {
                            Header = "Insert _Above",
                            Icon = IconProvider.GetControl(IconDescriptor.GetIcon(CalameIconKey.InsertAbove), 16)
                        };

                        CreateAddCommand(childrenSource, insertTarget: data).SetupMenuItem(insertAboveItem);
                        yield return insertAboveItem;
                    }

                    if (canInsertBelow)
                    {
                        var insertBelowItem = new MenuItem
                        {
                            Header = "_Insert Below",
                            Icon = IconProvider.GetControl(IconDescriptor.GetIcon(CalameIconKey.InsertBelow), 16)
                        };

                        CreateAddCommand(childrenSource, insertTarget: data, afterTarget: true).SetupMenuItem(insertBelowItem);
                        yield return insertBelowItem;
                    }

                    yield return new Separator();
                }

                if (data.ParentSource.CanRemoveAt(dataIndex))
                {
                    yield return new MenuItem
                    {
                        Header = "_Remove",
                        Command = _removeCommand,
                        CommandParameter = data,
                        Icon = IconProvider.GetControl(IconDescriptor.GetIcon(CalameIconKey.Delete), 16)
                    };
                }
            }
        }

        private bool CanRemove(object obj) => obj is IGlyphData item && item.ParentSource != null;
        private void OnRemove(object obj)
        {
            if (!CanRemove(obj))
                return;

            var child = (IGlyphData)obj;

            IGlyphDataSource childrenSource = child.ParentSource;
            int index = childrenSource.IndexOf(child);

            _undoRedoContext?.UndoRedoStack.Execute($"Remove data {child} from source {childrenSource}.",
                () =>
                {
                    Selection = child.ParentSource.Owner;
                    childrenSource.RemoveAt(index);
                    (child as IRestorable)?.Store();
                },
                () =>
                {
                    (child as IRestorable)?.Restore();
                    childrenSource.Insert(index, child);
                    Selection = child;
                },
                () => (child as IDisposable)?.Dispose(),
                null);
        }

        protected override Task OnDocumentActivated(IDocumentContext<IRootDataContext> activeDocument)
        {
            _selection = null;

            _selectionContext = activeDocument.GetSelectionContext<IGlyphData>();
            _undoRedoContext = activeDocument.TryGetContext<IUndoRedoContext>();
            RootDataContext = activeDocument.Context;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            _selection = null;

            RootDataContext = null;
            _undoRedoContext = null;
            _selectionContext = null;

            return Task.CompletedTask;
        }
        
        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            SetValue(ref _selection, message.Item, nameof(Selection));
            return Task.CompletedTask;
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            switch (data)
            {
                case IGlyphData glyphData:
                    return _dataItemBuilder.Build(glyphData, synchronizerConfiguration);
                case IGlyphDataChildrenSource childrenSource:
                    return _childrenSourceItemBuilder.Build(childrenSource, synchronizerConfiguration);
                default:
                    throw new NotSupportedException();
            }
        }

        public bool DisableChildrenIfParentDisabled => false;
        event EventHandler ITreeContext.BaseFilterChanged { add { } remove { } }
        bool ITreeContext.IsMatchingBaseFilter(object data) => true;
    }
}