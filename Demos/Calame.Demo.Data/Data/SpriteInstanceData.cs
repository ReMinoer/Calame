using Calame.Demo.Data.Data.Base;
using Calame.Demo.Data.Engine;
using Glyph.IO;
using Simulacra.Injection.Binding;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data.Data
{
    public class SpriteInstanceData : InstanceDataBase<SpriteInstanceData, SpriteInstanceObject>
    {
        [FileType(".png", ".jpg", DisplayName = "Image Files")]
        public AssetPath AssetPath { get; set; }

        static SpriteInstanceData()
        {
            Bindings.FromPath(x => x.AssetPath).To(x => x.SpriteLoader.AssetPath);
        }
    }
}