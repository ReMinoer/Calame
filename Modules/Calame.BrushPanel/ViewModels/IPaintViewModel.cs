using Glyph.Tools;
using Glyph.Tools.Brushing;

namespace Calame.BrushPanel.ViewModels
{
    public interface IPaintViewModel
    {
        string DisplayName { get; }
        object IconKey { get; }
        IPaint Paint { get; }
    }

    public interface IPaintViewModel<out TPaint> : IPaintViewModel
        where TPaint : IPaint
    {
        new TPaint Paint { get; }
    }
}