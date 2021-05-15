using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Fingear.Interactives;
using Glyph;
using Glyph.Composition;
using Glyph.Engine;
using Stave;

namespace Calame.Viewer
{
    public class DebuggableViewerContexts : PropertyChangedBase, IRootsContext, IRootComponentsContext, IRootScenesContext, IRootInteractivesContext
    {
        private ViewerViewModel _viewer;
        public ViewerViewModel Viewer
        {
            get => _viewer;
            private set => Set(ref _viewer, value);
        }
        
        private IEnumerable _roots;
        public IEnumerable Roots
        {
            get => _roots;
            private set => Set(ref _roots, value);
        }

        private IEnumerable<IGlyphComponent> _rootComponents;
        public IEnumerable<IGlyphComponent> RootComponents
        {
            get => _rootComponents;
            private set => Set(ref _rootComponents, value);
        }

        private IEnumerable<IInteractive> _rootInteractives;
        public IEnumerable<IInteractive> RootInteractives
        {
            get => _rootInteractives;
            private set => Set(ref _rootInteractives, value);
        }

        private bool _debugMode;
        public bool DebugMode
        {
            get => _debugMode;
            set
            {
                if (Set(ref _debugMode, value))
                    RefreshContexts();
            }
        }

        public GlyphEngine Engine => _viewer.Runner.Engine;
        public IEnumerable<ISceneNode> RootScenes => _viewer.Runner.Engine.ProjectionManager.SceneRoots;

        private IGlyphComponent _userParentComponent;

        public IGlyphComponent UserParentComponent
        {
            get => _userParentComponent;
            set
            {
                if (_userParentComponent == value)
                    return;

                _userParentComponent = value;

                if (!DebugMode)
                    RefreshContexts();
            }
        }

        public DebuggableViewerContexts(ViewerViewModel viewer, ISelectionCommandContext selectionCommandContext)
        {
            Viewer = viewer;
            SelectCommand = new SelectionCommand(selectionCommandContext);
        }

        public void RefreshContexts()
        {
            if (DebugMode)
            {
                RootComponents = new IGlyphComponent[] { _viewer.Runner.Engine.Root };
                RootInteractives = new IInteractive[] { _viewer.Runner.Engine.InteractionManager.Root };
            }
            else
            {
                RootComponents = (UserParentComponent ?? _viewer.UserRoot).Components;
                RootInteractives = _viewer.InteractiveModes.Where(x => x.IsUserMode).Select(x => x.Interactive);
            }

            Roots = RootComponents;
            CanSelectChanged?.Invoke(this, EventArgs.Empty);
        }

        public ICommand SelectCommand { get; }
        public event EventHandler CanSelectChanged;

        public bool CanSelect(IGlyphComponent component)
        {
            if (DebugMode)
                return CanSelectInDebugMode(component);

            return CanSelectInUserMode(component);
        }

        private bool CanSelectInDebugMode(IGlyphComponent component)
        {
            return CanSelectBase(component)
                && !component.AndAllParents().Any(_viewer.NotSelectableComponents.Contains);
        }

        private bool CanSelectInUserMode(IGlyphComponent component)
        {
            return CanSelectBase(component)
                && component.AllParents().Contains(UserParentComponent ?? _viewer.UserRoot);
        }

        private bool CanSelectBase(IGlyphComponent component)
        {
            return component != null && !component.GetType().IsValueType;
        }

        private class SelectionCommand : ICommand
        {
            private readonly ISelectionCommandContext _context;

            public SelectionCommand(ISelectionCommandContext context)
            {
                _context = context;
            }

            public event EventHandler CanExecuteChanged
            {
                add => _context.CanSelectChanged += value;
                remove => _context.CanSelectChanged -= value;
            }

            public bool CanExecute(object parameter) => _context.CanSelect(parameter);
            public void Execute(object parameter) => _context.SelectAsync(parameter).Wait();
        }
    }
}