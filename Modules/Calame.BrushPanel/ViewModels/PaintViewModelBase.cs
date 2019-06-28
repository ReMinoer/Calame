using System.ComponentModel;
using Glyph.Tools;
using Glyph.Tools.Brushing;

namespace Calame.BrushPanel.ViewModels
{
    public abstract class PaintViewModelBase<TPaint> : IPaintViewModel<TPaint>
        where TPaint : IPaint
    {
        [Browsable(false)]
        public abstract string DisplayName { get; }
        [Browsable(false)]
        public abstract object IconKey { get; }
        [Browsable(false)]
        public abstract TPaint Paint { get; }

        IPaint IPaintViewModel.Paint => Paint;
    }
}