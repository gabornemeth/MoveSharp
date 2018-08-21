//
// JsonExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Newtonsoft.Json
{
    /// <summary>
    /// Extension methods for Newtonsoft.Json
    /// </summary>
    public static class JsonExtensions
    {
        public static bool HasValue(this JObject obj, string propertyName)
        {
            JToken value;
            return obj.TryGetValue(propertyName, out value);
        }

        public static string SerializeAsJsonString(object obj, JsonSerializer serializer)
        {
            using (var stream = new MemoryStream())
            {
                // serialize into a stream
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 512, true))
                {
                    serializer.Serialize(writer, obj);
                }

                // read back the text
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string SerializeAsJsonString(this object obj)
        {
            return SerializeAsJsonString(obj, new JsonSerializer());
        }
    }
}
