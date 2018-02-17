using Microsoft.Xna.Framework.Content;

namespace Calame
{
    public interface IContentManagerProvider
    {
        ContentManager Get(string path);
        bool Remove(string path);
    }
}