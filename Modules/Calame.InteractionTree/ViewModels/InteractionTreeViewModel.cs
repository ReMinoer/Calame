using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Fingear.Controls;
using Fingear.Interactives;
using Gemini.Framework.Services;

namespace Calame.InteractionTree.ViewModels
{
    [Export(typeof(InteractionTreeViewModel))]
    public sealed class InteractionTreeViewModel : CalameTool<IDocumentContext<IRootInteractivesContext>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }

        private readonly TreeViewItemModelBuilder<IInteractive> _interactiveTreeItemBuilder;
        private readonly TreeViewItemModelBuilder<IControl> _controlTreeItemBuilder;

        private IRootInteractivesContext _rootInteractivesContext;
        public IRootInteractivesContext RootInteractivesContext
        {
            get => _rootInteractivesContext;
            private set => SetValue(ref _rootInteractivesContext, value);
        }

        protected override object IconKey => CalameIconKey.InteractionTree;

        [ImportingConstructor]
        public InteractionTreeViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Interaction Tree";

            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor();

            IIconDescriptor<IInteractive> interactiveIconDescriptor = iconDescriptorManager.GetDescriptor<IInteractive>();
            IIconDescriptor<IControl> controlIconDescriptor = iconDescriptorManager.GetDescriptor<IControl>();

            _interactiveTreeItemBuilder = new TreeViewItemModelBuilder<IInteractive>()
                                          .DisplayName(x => x.Name, nameof(IInteractive.Name))
                                          .ChildrenSource(x => new CompositeReadOnlyObservableList<object>
                                          (
                                              new EnumerableReadOnlyObservableList<object>(x.Components),
                                              new EnumerableReadOnlyObservableList<object>(x.Controls)
                                          ), x => ObservableHelpers.OnPropertyChanged(x as INotifyPropertyChanged, nameof(IInteractive.Components), nameof(IInteractive.Controls)))
                                          .IconDescription(x => interactiveIconDescriptor.GetIcon(x))
                                          .IsEnabled(x => x.Enabled, nameof(IInteractive.Enabled));

            _controlTreeItemBuilder = new TreeViewItemModelBuilder<IControl>()
                                      .DisplayName(x => x.Name, nameof(IControl.Name))
                                      .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Components), nameof(IControl.Components))
                                      .IconDescription(x => controlIconDescriptor.GetIcon(x))
                                      .IsTriggered(x => x.IsActive, nameof(IControl.IsActive));
        }
        
        protected override Task OnDocumentActivated(IDocumentContext<IRootInteractivesContext> activeDocument)
        {
            RootInteractivesContext = activeDocument.Context;
            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            RootInteractivesContext = null;
            return Task.CompletedTask;
        }

        public ITreeViewItemModel CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            switch (data)
            {
                case IInteractive interactive:
                    return _interactiveTreeItemBuilder.Build(interactive, synchronizerConfiguration);
                case IControl control:
                    return _controlTreeItemBuilder.Build(control, synchronizerConfiguration);
                default:
                    throw new NotSupportedException();
            }
        }

        public bool DisableChildrenIfParentDisabled => true;
        event EventHandler ITreeContext.BaseFilterChanged { add { } remove { } }
        bool ITreeContext.IsMatchingBaseFilter(object data) => true;
    }
}