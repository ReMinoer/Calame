using Calame.Demo.Data.Engine;
using Diese.Collections.Observables;
using Glyph.Composition.Modelization;
using Simulacra.Binding;

namespace Calame.Demo.Data.Data
{
    public class SceneData : BindedData<SceneData, Scene>
    {
        public ObservableCollection<InstanceData> Instances { get; } = new ObservableCollection<InstanceData>();

        static SceneData()
        {
            Bindings.FromCollection(x => x.Instances).ToBindedComposite();
        }
    }
}