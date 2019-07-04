using Newtonsoft.Json;
using System.IO;

namespace MarkdownExtension.EnterpriseArchitect
{
    internal static class JsonSerializerExtensions
    {
        public static void SerializeToFile(this JsonSerializer jsonSerializer, object obj, string fileName)
        {
            using (var streamWriter = File.CreateText(fileName))
            {
                jsonSerializer.Serialize(streamWriter, obj);
            }
        }
        public static T DeserializeFromFile<T>(this JsonSerializer jsonSerializer, string fileName)
        {
            using (var file = File.OpenText(fileName))
            using (var jsonReader = new JsonTextReader(file))
            {
                return jsonSerializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
