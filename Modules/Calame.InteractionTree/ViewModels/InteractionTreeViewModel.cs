using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables.ReadOnly;
using Fingear;
using Gemini.Framework.Services;
using Glyph.Engine;

namespace Calame.InteractionTree.ViewModels
{
    [Export(typeof(InteractionTreeViewModel))]
    public sealed class InteractionTreeViewModel : CalameTool<IDocumentContext<GlyphEngine>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }

        private GlyphEngine _engine;
        private readonly TreeViewItemModelBuilder<IInteractive> _interactiveTreeItemBuilder;
        private readonly TreeViewItemModelBuilder<IControl> _controlTreeItemBuilder;

        public GlyphEngine Engine
        {
            get => _engine;
            private set => SetValue(ref _engine, value);
        }

        [ImportingConstructor]
        public InteractionTreeViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator)
        {
            DisplayName = "Interaction Tree";

            IconProvider = iconProvider;
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
        
        protected override Task OnDocumentActivated(IDocumentContext<GlyphEngine> activeDocument)
        {
            Engine = activeDocument.Context;
            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            Engine = null;
            return Task.CompletedTask;
        }

        public ITreeViewItemModel CreateTreeItemModel(object data, Func<object, ITreeViewItemModel> dataConverter, Action<ITreeViewItemModel> itemDisposer)
        {
            switch (data)
            {
                case IInteractive interactive:
                    return _interactiveTreeItemBuilder.Build(interactive, dataConverter, itemDisposer);
                case IControl control:
                    return _controlTreeItemBuilder.Build(control, dataConverter, itemDisposer);
                default:
                    throw new NotSupportedException();
            }
        }

        bool ITreeContext.BaseFilter(object data) => true;
    }
}