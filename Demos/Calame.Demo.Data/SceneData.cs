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
        public FilePath MainShapePath { get; set; }
        public ObservableCollection<IShapeData<IGlyphComponent>> Shapes { get; } = new ObservableCollection<IShapeData<IGlyphComponent>>();

        static SceneData()
        {
            PathBindings.From(x => x.MainShapePath)
                .Load(() => new DataContractSerializationFormat<IShapeData<IGlyphComponent>>())
                .CreateComponent()
                .To(x => x.MainShape);

            CollectionBindings.From(x => x.Shapes).ToBindedComposite();
        }
    }
}