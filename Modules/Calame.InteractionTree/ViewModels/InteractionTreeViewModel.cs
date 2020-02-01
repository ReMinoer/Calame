using System;
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
        private readonly IIconDescriptor<IInteractive> _interactiveIconDescriptor;
        private readonly IIconDescriptor<IControl> _controlIconDescriptor;
        
        private GlyphEngine _engine;
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
            _interactiveIconDescriptor = iconDescriptorManager.GetDescriptor<IInteractive>();
            _controlIconDescriptor = iconDescriptorManager.GetDescriptor<IControl>();
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

        public ITreeViewItemModel CreateTreeItemModel(object data)
        {
            switch (data)
            {
                case IInteractive interactive:
                    return new TreeViewItemModel<IInteractive>(
                        this,
                        interactive,
                        x => x.Name,
                        x => new CompositeReadOnlyObservableList<object>
                        (
                            new EnumerableReadOnlyObservableList<object>(x.Components),
                            new EnumerableReadOnlyObservableList<object>(x.Controls)
                        ),
                        _interactiveIconDescriptor.GetIcon(interactive),
                        nameof(IInteractive.Name),
                        nameof(IInteractive.Components))
                    {
                        IsEnabled = interactive.Enabled
                    };
                case IControl control:
                    return new TreeViewItemModel<IControl>(
                        this,
                        control,
                        x => x.Name,
                        x => new EnumerableReadOnlyObservableList<object>(x.Components),
                        _controlIconDescriptor.GetIcon(control),
                        nameof(IControl.Name),
                        nameof(IControl.Components),
                        isTriggeredFunc: x => x.IsActive,
                        isTriggeredPropertyName: nameof(IControl.IsActive));
                default:
                    throw new NotSupportedException();
            }
        }

        bool ITreeContext.BaseFilter(object data) => true;
    }
}