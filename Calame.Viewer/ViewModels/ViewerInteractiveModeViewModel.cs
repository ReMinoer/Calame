using Caliburn.Micro;

namespace Calame.Viewer.ViewModels
{
    public class ViewerInteractiveModeViewModel : PropertyChangedBase
    {
        public IViewerInteractiveMode InteractiveModel { get; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        public ViewerInteractiveModeViewModel(IViewerInteractiveMode interactiveModel)
        {
            InteractiveModel = interactiveModel;
        }
    }
}