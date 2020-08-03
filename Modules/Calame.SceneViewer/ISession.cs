using Glyph;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.SceneViewer
{
    public interface ISession
    {
        string DisplayName { get; }
        IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService);
        void PrepareSession(ISessionContext context);
    }
}