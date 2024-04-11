using System.IO;
using System.Reflection;

namespace Trxer
{
    internal static class ResourceReader
    {
        internal static string LoadTextFromResource(string name)
        {
            using StreamReader sr = new StreamReader(StreamFromResource(name));
            var result = sr.ReadToEnd();
            return result;
        }

        internal static Stream StreamFromResource(string name)
        {           
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Trxer." + name);
        }
    }
}
