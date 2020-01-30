using System.Collections.Generic;
using Calame.Icons;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Tools.Brushing;
using Glyph.Tools.Brushing.Space;

namespace Calame.BrushPanel.ViewModels
{
    public interface IBrushViewModel : IBrush
    {
        string DisplayName { get; }
        IconDescription IconDescription { get; }
        bool IsValidForCanvas(object canvas);
        IEnumerable<IPaintViewModel> Paints { get; }
    }

    public interface IEngineBrushViewModel : IBrushViewModel, IBrush<IGlyphComponent, ISpaceBrushArgs, IPaint>
    {
    }

    public interface IDataBrushViewModel : IBrushViewModel, IBrush<IGlyphData, ISpaceBrushArgs, IPaint>
    {
    }
}