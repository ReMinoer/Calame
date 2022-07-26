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
                .ContextMenuItems(GetContextMenuItems);

            _childrenSourceItemBuilder = new TreeViewItemModelBuilder<IGlyphDataChildrenSource>()
                .DisplayName(x => x.PropertyName)
                .FontWeight(_ => FontWeights.Bold)
                .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Children), nameof(IGlyphDataChildrenSource.Children))
                .IconDescription(x => IconDescriptor.GetIcon(x.Children), nameof(IGlyphDataChildrenSource.Children))
                .IsHeader(_ => true)
                .QuickCommand(x => CreateAddCommand(x), nameof(IGlyphDataChildrenSource.Children))
                .QuickCommandIconDescription(_ => new IconDescription(CalameIconKey.Add, Brushes.DarkGreen, margin: 0.5))
                .QuickCommandLabel(_ => "Add")
                .QuickCommandToolTip(_ => "Add");
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
                    childrenSource.Set(index, child);
                    Selection = child;
                },
                () =>
                {
                    Selection = child.ParentSource.Owner;
                    childrenSource.Unset(index);
                    (child as IRestorable)?.Store();
                },
                null,
                () => (child as IDisposable)?.Dispose());
        }

        private IEnumerable GetContextMenuItems(IGlyphData data)
        {
            if (data.ParentSource != null)
            {
                if (data.ParentSource is IGlyphDataChildrenSource childrenSource)
                {
                    var insertAboveItem = new MenuItem
                    {
                        Header = "Insert _Above",
                        Icon = IconProvider.GetControl(IconDescriptor.GetIcon(CalameIconKey.InsertAbove), 16)
                    };

                    var insertBelowItem = new MenuItem
                    {
                        Header = "_Insert Below",
                        Icon = IconProvider.GetControl(IconDescriptor.GetIcon(CalameIconKey.InsertBelow), 16)
                    };

                    CreateAddCommand(childrenSource, insertTarget: data).SetupMenuItem(insertAboveItem);
                    CreateAddCommand(childrenSource, insertTarget: data, afterTarget: true).SetupMenuItem(insertBelowItem);

                    yield return insertAboveItem;
                    yield return insertBelowItem;
                    yield return new Separator();
                }

                yield return new MenuItem
                {
                    Header = "_Remove",
                    Command = _removeCommand,
                    CommandParameter = data,
                    Icon = IconProvider.GetControl(IconDescriptor.GetIcon(CalameIconKey.Delete), 16)
                };
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
                    childrenSource.Unset(index);
                    (child as IRestorable)?.Store();
                },
                () =>
                {
                    (child as IRestorable)?.Restore();
                    childrenSource.Set(index, child);
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