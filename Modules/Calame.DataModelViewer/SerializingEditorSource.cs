using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.DataModelViewer
{
    public abstract class SerializingEditorSource<T, TEditor> : EditorSource<T>
        where T : IGlyphCreator, new()
        where TEditor : SerializingViewerEditorBase<T>, new()
    {
        private readonly IImportedTypeProvider _importedTypeProvider;

        protected abstract ISerializationFormat<T> SerializationFormat { get; }
        protected override sealed ISaveLoadFormat<T> SaveLoadFormat => SerializationFormat;

        protected SerializingEditorSource(IImportedTypeProvider importedTypeProvider)
        {
            _importedTypeProvider = importedTypeProvider;
        }
        
        public override IEditor CreateInstance()
        {
            return new TEditor
            {
                ImportedTypeProvider = _importedTypeProvider,
                SerializationFormat = SerializationFormat
            };
        }
    }
}