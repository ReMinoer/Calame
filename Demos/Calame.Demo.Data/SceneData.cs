using Diese.Collections.Observables;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.IO;
using Simulacra.Binding;
using Simulacra.Injection.Binding;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data
{
    public class SceneData : BindedData<SceneData, Scene>
    {
        public bool[] Array { get; set; } = new bool[5];
        public FilePath MainShapePath { get; set; }
        public ObservableList<FilePath> PathList { get; } = new ObservableList<FilePath>();
        public ObservableCollection<IShapeData<IGlyphComponent>> Shapes { get; } = new ObservableCollection<IShapeData<IGlyphComponent>>();
        public ObservableCollection<SceneData> SubScenes { get; } = new ObservableCollection<SceneData>();

        static SceneData()
        {
            PathBindings.From(x => x.MainShapePath)
                .Load(() => new DataContractSerializationFormat<IShapeData<IGlyphComponent>>())
                .CreateComponent()
                .To(x => x.MainShape);

            CollectionBindings.From(x => x.Shapes).ToBindedComposite();
            CollectionBindings.From(x => x.SubScenes).ToBindedComposite();
        }
    }
}