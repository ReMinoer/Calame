using System.IO;
using System.Threading.Tasks;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.DataModelViewer
{
    public abstract class SerializingViewerEditorBase<T> : ViewerEditorBase<T>
        where T : IGlyphCreator, new()
    {
        private readonly IImportedTypeProvider _importedTypeProvider;
        protected abstract ISerializationFormat<T> SerializationFormat { get; }
        protected override sealed ISaveLoadFormat<T> SaveLoadFormat => SerializationFormat;

        protected SerializingViewerEditorBase(IImportedTypeProvider importedTypeProvider)
        {
            _importedTypeProvider = importedTypeProvider;
        }

        public override Task<IGlyphCreator> NewDataAsync()
        {
            return Task.Run<IGlyphCreator>(() => new T());
        }

        public override Task<IGlyphCreator> LoadDataAsync(Stream stream)
        {
            return Task.Run<IGlyphCreator>(() => SerializationFormat.Load(stream, _importedTypeProvider.Types));
        }

        public override Task SaveDataAsync(object obj, Stream stream)
        {
            return Task.Run(() => SerializationFormat.Save((T)obj, stream, _importedTypeProvider.Types));
        }
    }
}