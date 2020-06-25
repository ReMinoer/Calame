using Calame.Demo.Data.Engine;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core.Modelization;
using Glyph.IO;
using Glyph.Tools.Transforming;
using Microsoft.Xna.Framework;
using Simulacra.Binding;
using Simulacra.Injection.Binding;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data.Data
{
    public class InstanceData : BindedData<InstanceData, InstanceObject>, IPositionController
    {
        public Vector2 LocalPosition { get; set; }
        public FilePath FilePath { get; set; }

        static InstanceData()
        {
            Bindings.From(x => x.LocalPosition).To(x => x.SceneNode.LocalPosition);
            Bindings.FromPath(x => x.FilePath)
                .Load(() => new DataContractSerializationFormat<IGlyphCreator<IGlyphComponent>>())
                .CreateComponent()
                .SetToBindedObject();
        }

        Vector2 IPositionController.Position
        {
            get => LocalPosition;
            set => LocalPosition = value;
        }
    }
}