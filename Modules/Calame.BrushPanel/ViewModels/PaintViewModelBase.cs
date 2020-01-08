using System.ComponentModel;
using Glyph.Tools.Brushing;

namespace Calame.BrushPanel.ViewModels
{
    public abstract class PaintViewModelBase : IPaintViewModel, IPaint
    {
        [Browsable(false)]
        public abstract string DisplayName { get; }
        [Browsable(false)]
        public abstract object IconKey { get; }
        [Browsable(false)]
        IPaint IPaintViewModel.Paint => this;
    }
}