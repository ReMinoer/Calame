using System.IO;
using System.Threading.Tasks;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.DataModelViewer
{
    public abstract class SerializingViewerEditorBase<T> : ViewerEditorBase<T>
        where T : IGlyphCreator, new()
    {
        public IImportedTypeProvider ImportedTypeProvider { get; set; }
        public ISerializationFormat<T> SerializationFormat { get; set; }

        protected override sealed ISaveLoadFormat<T> SaveLoadFormat => SerializationFormat;

        protected override Task<T> NewAsync()
        {
            return Task.FromResult(new T());
        }

        protected override Task<T> LoadAsync(Stream stream)
        {
            return Task.Run(() =>
                {
                    SerializationFormat.KnownTypes = ImportedTypeProvider.Types;
                    return SerializationFormat.Load(stream);
                }
            );
        }

        protected override Task SaveAsync(T data, Stream stream)
        {
            return Task.Run(() =>
                {
                    SerializationFormat.KnownTypes = ImportedTypeProvider.Types;
                    SerializationFormat.Save(data, stream);
                }
            );
        }
    }
}