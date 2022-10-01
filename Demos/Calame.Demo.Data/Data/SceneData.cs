using Calame.Demo.Data.Engine;
using Diese.Collections.Observables;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Simulacra.Binding;

namespace Calame.Demo.Data.Data
{
    public class SceneData : BindedData<SceneData, Scene>
    {
        public ObservableList<IInstanceData<IGlyphComponent>> Instances { get; } = new ObservableList<IInstanceData<IGlyphComponent>>();

        static SceneData()
        {
            Bindings.FromCollection(x => x.Instances).ToBindedComposite();
        }
    }
}