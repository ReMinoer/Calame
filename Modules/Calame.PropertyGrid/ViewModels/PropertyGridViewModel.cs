using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : HandleTool, IHandle<ISelection<object>>
    {
        private object _selectedObject;
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public object SelectedObject
        {
            get => _selectedObject;
            set => SetValue(ref _selectedObject, value);
        }

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DisplayName = "Property Grid";
        }

        public void Handle(ISelection<object> message) => SelectedObject = message.Item;
    }
}