using System.Net.Mime;

namespace Utils.Sys.RichHttpClient
{
    public static class ContentTypeProvider
    {

        public static ContentType Json { get; } = new("application/json");

        public static ContentType Xml { get; } = new("application/xml");

        //private static readonly ConcurrentDictionary<string, ContentType> ContentTypes = new ConcurrentDictionary<string, ContentType>(
        //    Resources.Resources
        //    .FileExtensions
        //    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
        //    .Skip(1)
        //    .Select(x => x.Split(':').Select(p => p.Trim(' ', '"')).ToArray())
        //    .Select(x => KeyValuePair.Create(x[0].ToLower(), new ContentType(x[1]))));

        //public static ContentType Json { get; } = GetContentTypeByExtension("json");

        //public static ContentType Text { get; } = GetContentTypeByExtension("txt");

        //public static ContentType Xml { get; } = GetContentTypeByExtension("xml");

        //public static ContentType Pdf { get; } = GetContentTypeByExtension("pdf");

        //public static ContentType Xlsx { get; } = GetContentTypeByExtension("xlsx");

        //public static ContentType Stream { get; } = GetContentTypeByExtension("octet-stream");

        //public static ContentType AnyType { get; } = new ContentType("*/*");

        //public static ContentType GetContentTypeByExtension(string extension)
        //{
        //    extension = extension.ToLower().TrimStart('.');
        //    return ContentTypes.GetValueOrDefault(extension) ?? ContentTypes.GetValueOrDefault("*");
        //}
    }
}
