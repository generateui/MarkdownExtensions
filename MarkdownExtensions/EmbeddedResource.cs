using System.IO;
using System.Reflection;

namespace MarkdownExtensions
{
    public static class EmbeddedResource
    {
        public static string GetFileContent(this Assembly assembly, string resourceUri)
        {
            string path = $@"{assembly.GetName().Name}.{resourceUri}";
            using (Stream stream = assembly.GetManifestResourceStream(path))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
