using Calame.Demo.Data.Data.Base;
using Calame.Demo.Data.Engine;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core.Modelization;
using Glyph.IO;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data.Data
{
    public class FileInstanceData : InstanceDataBase<FileInstanceData, InstanceObject>
    {
        [FileContent(typeof(IGlyphCreator))]
        public FilePath FilePath { get; set; }

        static FileInstanceData()
        {
            Bindings.FromPath(x => x.FilePath)
                .Load(() => new DataContractSerializationFormat<IGlyphCreator<IGlyphComponent>>())
                .CreateComponent()
                .SetToBindedObject();
        }
    }
}