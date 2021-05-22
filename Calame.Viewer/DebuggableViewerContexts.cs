using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calame.DocumentContexts;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Fingear.Interactives;
using Glyph;
using Glyph.Composition;
using Glyph.Pipeline;
using Glyph.WpfInterop;
using Stave;

namespace Calame.Viewer
{
    public class DebuggableViewerContexts : PropertyChangedBase, IRawContentLibraryContext, IRootsContext, IRootComponentsContext, IRootScenesContext, IRootInteractivesContext
    {
        public ViewerViewModel Viewer { get; }

        private IContentLibrary _contentLibrary;
        public IContentLibrary ContentLibrary
        {
            get => _contentLibrary;
            private set => Set(ref _contentLibrary, value);
        }

        private IRawContentLibrary _rawContentLibrary;
        public IRawContentLibrary RawContentLibrary
        {
            get => _rawContentLibrary;
            private set => Set(ref _rawContentLibrary, value);
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

        private IEnumerable<ISceneNode> _rootScenes;
        public IEnumerable<ISceneNode> RootScenes
        {
            get => _rootScenes;
            private set => Set(ref _rootScenes, value);
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
                    RefreshDebuggableContexts();
            }
        }

        private IGlyphComponent _userParentComponent;
        public IGlyphComponent UserParentComponent
        {
            get => _userParentComponent;
            set
            {
                if (Set(ref _userParentComponent, value) && !DebugMode)
                    RefreshDebuggableContexts();
            }
        }

        public DebuggableViewerContexts(ViewerViewModel viewer)
        {
            Viewer = viewer;

            RefreshRunner();
            Viewer.RunnerChanged += OnRunnerChanged;
        }

        private void OnRunnerChanged(object sender, GlyphWpfRunner e) => RefreshRunner();
        private void RefreshRunner()
        {
            ContentLibrary = Viewer.Runner?.Engine.ContentLibrary;
            RawContentLibrary = ContentLibrary as IRawContentLibrary;
            RootScenes = Viewer.Runner?.Engine.ProjectionManager.SceneRoots;

            RefreshDebuggableContexts();
        }

        public void RefreshDebuggableContexts()
        {
            if (Viewer.Runner == null)
            {
                RootComponents = Enumerable.Empty<IGlyphComponent>();
                RootInteractives = Enumerable.Empty<IInteractive>();
            }
            else if (DebugMode)
            {
                RootComponents = new IGlyphComponent[] { Viewer.Runner.Engine.Root };
                RootInteractives = new IInteractive[] { Viewer.Runner.Engine.InteractionManager.Root };
            }
            else
            {
                RootComponents = (UserParentComponent ?? Viewer.UserRoot).Components;
                RootInteractives = Viewer.InteractiveModes.Where(x => x.IsUserMode).Select(x => x.Interactive);
            }

            Roots = RootComponents;
            CanSelectChanged?.Invoke(this, EventArgs.Empty);
        }
        
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
                && !component.AndAllParents().Any(Viewer.NotSelectableComponents.Contains);
        }

        private bool CanSelectInUserMode(IGlyphComponent component)
        {
            return CanSelectBase(component)
                && component.AllParents().Contains(UserParentComponent ?? Viewer.UserRoot);
        }

        private bool CanSelectBase(IGlyphComponent component)
        {
            return component != null && !component.GetType().IsValueType;
        }
    }
}