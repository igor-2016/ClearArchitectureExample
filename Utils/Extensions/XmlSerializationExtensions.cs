using System.Xml;
using System.Xml.Serialization;

namespace Utils.Sys.Extensions
{
    public static class XmlSerializationExtensions
    {
        public static T DeserializeXml<T>(this string serializedDocument)
        {
            if (serializedDocument == null)
            {
                return default;
            }

            var doc = new XmlDocument();
            doc.LoadXml(serializedDocument);

            return doc.DeserializeXml<T>();
        }

        public static T DeserializeXml<T>(this XmlDocument serializedDocument)
        {
            if (serializedDocument == null)
            {
                return default;
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var nodeReader = new XmlNodeReader(serializedDocument);

            var deserializedValue = (T)serializer.Deserialize(nodeReader);

            return deserializedValue;
        }

        public static string SerializationXml<T>(this T model)
        {
            using var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, model);
            return stringwriter.ToString();
        }
    }

}
