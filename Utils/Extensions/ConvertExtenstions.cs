using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Utils
{


    //TODO merge with Serialize extensions!
    public static class ConvertExtenstions
    {
        public static T XmlDeserialize<T>(this string toDeserialize)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var textReader = new StringReader(toDeserialize))
            {
                if(xmlSerializer.Deserialize(textReader) is T obj)
                    return obj;

                throw new InvalidOperationException($"Deserialization error: object type is not {typeof(T)}");
            }
        }

        public static string XmlSerialize<T>(this T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var textWriter = new StringWriter()) // Utf8StringWriter()) ?
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T? JsonDeserialize<T>(this string toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }

        public static string JsonSerialize<T>(this T toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }
    }

}
