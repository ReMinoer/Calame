using Calame.Icons;
using Glyph.Tools.Brushing;

namespace Calame.BrushPanel.ViewModels
{
    public interface IPaintViewModel
    {
        string DisplayName { get; }
        IconDescription IconDescription { get; }
        IPaint Paint { get; }
    }

    public interface IPaintViewModel<out TPaint> : IPaintViewModel
        where TPaint : IPaint
    {
        new TPaint Paint { get; }
    }
}