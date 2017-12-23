using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Engine;
using Glyph.Modelization;

namespace Calame.DataModelViewer
{
    public interface IEditor
    {
        string DisplayName { get; }
        IEnumerable<string> FileExtensions { get; }
        string ContentPath { get; }
        IGlyphComposite<IGlyphComponent> PrepareEditor(GlyphEngine engine);
        Task<IBindedGlyphCreator<IGlyphComponent>> NewDataAsync();
        Task<IBindedGlyphCreator<IGlyphComponent>> LoadDataAsync(Stream stream);
        Task SaveDataAsync(object obj, Stream stream);
    }
}