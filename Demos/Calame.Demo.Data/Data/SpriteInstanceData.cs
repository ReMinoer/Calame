using Calame.Demo.Data.Data.Base;
using Calame.Demo.Data.Engine;
using Glyph.IO;
using Microsoft.Xna.Framework.Graphics;
using Simulacra.Injection.Binding;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data.Data
{
    public class SpriteInstanceData : InstanceDataBase<SpriteInstanceData, SpriteInstanceObject>
    {
        [FileContent(typeof(Texture2D))]
        public AssetPath AssetPath { get; set; }

        static SpriteInstanceData()
        {
            Bindings.FromPath(x => x.AssetPath).To(x => x.SpriteLoader.AssetPath);
        }
    }
}