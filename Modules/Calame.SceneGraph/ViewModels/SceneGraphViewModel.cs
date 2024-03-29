﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;

namespace Calame.SceneGraph.ViewModels
{
    [Export(typeof(SceneGraphViewModel))]
    public sealed class SceneGraphViewModel : CalameTool<IDocumentContext<IRootScenesContext>>, ITreeContext,
        IHandle<ISelectionSpread<IGlyphComponent>>,
        IHandle<ISelectionSpread<IGlyphData>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }

        private IRootScenesContext _rootScenesContext;
        public IRootScenesContext RootScenesContext
        {
            get => _rootScenesContext;
            set => Set(ref _rootScenesContext, value);
        }
        
        private IUndoRedoContext _undoRedoContext;
        private ISelectionContext<IGlyphComponent> _selectionContext;
        private ICommand _selectionCommand;

        private IGlyphComponent _selection;
        public IGlyphComponent Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                _selectionNode = _selection?.GetSceneNode();
                NotifyOfPropertyChange(nameof(SelectionNode));

                _selectionContext.SelectAsync(_selection).Wait();
            }
        }

        private SceneNode _selectionNode;
        public SceneNode SelectionNode
        {
            get => _selectionNode;
            set
            {
                if (!SetValue(ref _selectionNode, value))
                    return;

                _selection = _selectionNode?.Parent;
                NotifyOfPropertyChange(nameof(Selection));

                _selectionContext.SelectAsync(_selection).Wait();
            }
        }

        private readonly TreeViewItemModelBuilder<ISceneNode> _treeItemBuilder;

        protected override object IconKey => CalameIconKey.SceneGraph;

        [ImportingConstructor]
        public SceneGraphViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Scene Graph";
            
            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor();

            IIconDescriptor<IGlyphComponent> iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();

            _treeItemBuilder = new TreeViewItemModelBuilder<ISceneNode>()
                               .DisplayName(x => (x as IGlyphComponent)?.Parent?.Name ?? x.ToString(), nameof(IGlyphComponent.Name), x => (x as IGlyphComponent)?.Parent as INotifyPropertyChanged)
                               .CanEditDisplayName(_ => true)
                               .DisplayNameSetter(x =>
                                   newName =>
                                   {
                                       IGlyphContainer glyphContainer = (x as IGlyphComponent)?.Parent;
                                       if (glyphContainer != null)
                                           glyphContainer.Name = newName;
                                   })
                               .ChildrenSource(x => x.Children, nameof(ISceneNode.Children))
                               .IconDescription(x => iconDescriptor.GetIcon((x as IGlyphComponent)?.Parent ?? x as IGlyphComponent))
                               .IsEnabled(x => _selectionCommand, x => (x as IGlyphComponent)?.Parent);
        }

        protected override Task OnDocumentActivated(IDocumentContext<IRootScenesContext> activeDocument)
        {
            _selection = null;
            _selectionNode = null;
            
            _undoRedoContext = activeDocument.TryGetContext<IUndoRedoContext>();
            _selectionContext = activeDocument.GetSelectionContext<IGlyphComponent>();
            _selectionCommand = _selectionContext.GetSelectionCommand();

            RootScenesContext = activeDocument.Context;

            if (_selectionContext != null)
                _selectionContext.CanSelectChanged += OnCanSelectChanged;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            if (_selectionContext != null)
                _selectionContext.CanSelectChanged -= OnCanSelectChanged;

            _selection = null;
            _selectionNode = null;

            RootScenesContext = null;

            _selectionCommand = null;
            _selectionContext = null;
            _undoRedoContext = null;

            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphComponent>>.HandleAsync(ISelectionSpread<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item?.BindedObject);
            return Task.CompletedTask;
        }

        private void HandleSelection(IGlyphComponent component)
        {
            SetValue(ref _selection, component, nameof(Selection));
            SetValue(ref _selectionNode, component?.GetSceneNode(), nameof(SelectionNode));
        }

        public bool DisableChildrenIfParentDisabled => false;

        bool ITreeContext.IsMatchingBaseFilter(object data)
        {
            return _selectionContext == null || _selectionContext.CanSelect((data as IGlyphComponent)?.Parent);
        }

        private event EventHandler BaseFilterChanged;
        event EventHandler ITreeContext.BaseFilterChanged
        {
            add => BaseFilterChanged += value;
            remove => BaseFilterChanged -= value;
        }

        private void OnCanSelectChanged(object sender, EventArgs e)
        {
            BaseFilterChanged?.Invoke(this, EventArgs.Empty);
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            return _treeItemBuilder.Build((ISceneNode)data, synchronizerConfiguration, _undoRedoContext?.UndoRedoStack);
        }
    }
}