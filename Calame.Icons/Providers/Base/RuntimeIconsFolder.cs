using System.IO;

namespace Calame.Icons.Providers.Base
{
    static public class RuntimeIconsFolder
    {
        static public readonly string Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CalameMahAppsRuntimeIcons");

        static public void Clean()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }

        static public void Create()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }
    }
}