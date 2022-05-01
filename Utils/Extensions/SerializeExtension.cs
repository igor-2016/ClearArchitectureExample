using Newtonsoft.Json;
using System.Xml.Serialization;
using Utils.Attributes;

namespace Utils
{
    public static partial class SerializeExtension
    {

        public static T ToObj<T>(this string xmlOrJson, string mediaType)
        {
            if (string.IsNullOrEmpty(xmlOrJson))
            {
                return default;
            }

            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentException("mediaType is null");
            }

            if (mediaType.Contains("json", StringComparison.InvariantCultureIgnoreCase))
            {
                return xmlOrJson.FromJson<T>();
            }
            else if (mediaType.Contains("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                return xmlOrJson.FromXml<T>();
            }

            throw new ArgumentException($"Unknown media type {mediaType}");
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(this string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static string ToXmlUtf8<T>(this T obj)
        {
            using (var stringwriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
        }

        public static T FromXml<T>(this string content)
        {
            using (var reader = new StringReader(content))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }



    }
}
