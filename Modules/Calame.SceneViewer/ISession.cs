using Glyph;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.SceneViewer
{
    public interface ISession
    {
        string DisplayName { get; }
        IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger);
        void PrepareSession(ISessionContext context);
    }
}