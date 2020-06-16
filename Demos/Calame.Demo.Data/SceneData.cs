using Diese.Collections.Observables;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.IO;
using Microsoft.Xna.Framework;
using Simulacra.Binding;
using Simulacra.Injection.Binding;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data
{
    public class SceneData : BindedData<SceneData, Scene>
    {
        public ObservableList<bool> Booleans { get; set; } = new ObservableList<bool> { false, true, false, true, false };
        public ObservableList<int> Ints { get; set; } = new ObservableList<int>();
        public ObservableList<float> Floats { get; set; } = new ObservableList<float>();
        public ObservableList<Vector2> Vectors { get; set; } = new ObservableList<Vector2>();

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