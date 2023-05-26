using System.Diagnostics;
using System.Reflection;

namespace Calame
{
    static public class CalameUtils
    {
        static public bool IsDevelopmentBuild() => IsDevelopmentBuild(GetCurrentExecutablePath());
        static public bool IsDevelopmentBuild(string executablePath) => GetVersion(executablePath) is null;

        static public string GetVersion() => GetVersion(GetCurrentExecutablePath());
        static public string GetVersion(string executablePath)
        {
            string fileVersion = FileVersionInfo.GetVersionInfo(executablePath).FileVersion;
            string executableVersion = fileVersion.Substring(0, fileVersion.LastIndexOf('.'));

            return executableVersion != "1.0.0" ? executableVersion : null;
        }

        static public string GetCurrentExecutablePath() => Assembly.GetEntryAssembly().Location;
    }
}