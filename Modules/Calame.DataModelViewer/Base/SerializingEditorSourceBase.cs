using System.Collections.Generic;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.DataModelViewer.Base
{
    public abstract class SerializingEditorSourceBase<T, TEditor> : EditorSourceBase<T>
        where T : IGlyphCreator, new()
        where TEditor : SerializingViewerEditorBase<T>, new()
    {
        private readonly IImportedTypeProvider _importedTypeProvider;
        protected abstract ISerializationFormat SerializationFormat { get; }

        protected SerializingEditorSourceBase(IImportedTypeProvider importedTypeProvider, string displayName, IEnumerable<string> extensions)
            : base(displayName, extensions)
        {
            _importedTypeProvider = importedTypeProvider;
        }

        protected SerializingEditorSourceBase(IImportedTypeProvider importedTypeProvider, string displayName, params string[] extensions)
            : base(displayName, extensions)
        {
            _importedTypeProvider = importedTypeProvider;
        }

        public override IEditor CreateEditor()
        {
            return new TEditor
            {
                ImportedTypeProvider = _importedTypeProvider,
                SerializationFormat = SerializationFormat
            };
        }
    }
}