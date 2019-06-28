using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Tools;
using Glyph.Tools.Brushing;
using Glyph.Tools.Brushing.Space;

namespace Calame.BrushPanel.ViewModels
{
    public interface IBrushViewModel : IBrush<IGlyphComponent, ISpaceBrushArgs, IPaint>
    {
        string DisplayName { get; }
        object IconKey { get; }
        Type CanvasType { get; }
        IBrush Brush { get; }
        IEnumerable<IPaintViewModel> Paints { get; }
    }
}