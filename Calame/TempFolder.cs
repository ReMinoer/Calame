using System.IO;

namespace Calame
{
    static public class TempFolder
    {
        static public readonly string Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CalameTemp");

        static TempFolder()
        {
            Clean();
        }

        static public void Clean()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }

        static public void CreateIfMissing()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }
    }
}