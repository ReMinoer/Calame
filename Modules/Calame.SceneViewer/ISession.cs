using System.Threading.Tasks;
using Glyph;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.SceneViewer
{
    public interface ISession
    {
        string DisplayName { get; }
        IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger);
        Task PrepareSessionAsync(ISessionContext sessionContext);
        Task ResetSessionAsync(ISessionContext sessionContext);
    }
}