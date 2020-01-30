using System.ComponentModel;
using Calame.Icons;
using Glyph.Tools.Brushing;

namespace Calame.BrushPanel.ViewModels
{
    public abstract class PaintViewModelBase : IPaintViewModel, IPaint
    {
        [Browsable(false)]
        public abstract string DisplayName { get; }
        [Browsable(false)]
        public abstract IconDescription IconDescription { get; }
        [Browsable(false)]
        IPaint IPaintViewModel.Paint => this;
    }
}