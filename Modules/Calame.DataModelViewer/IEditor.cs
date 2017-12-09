﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Engine;

namespace Calame.DataModelViewer
{
    public interface IEditor
    {
        string DisplayName { get; }
        IEnumerable<string> FileExtensions { get; }
        string ContentPath { get; }
        IGlyphComposite<IGlyphComponent> PrepareEditor(GlyphEngine engine);
        Task<IGlyphCreator<IGlyphComponent>> NewDataAsync();
        Task<IGlyphCreator<IGlyphComponent>> LoadDataAsync(Stream stream);
        Task SaveDataAsync(object obj, Stream stream);
    }
}