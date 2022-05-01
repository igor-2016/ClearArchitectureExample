using Newtonsoft.Json;

namespace Utils
{
    // TODO move to Utils
    public static class DataExtensions
    {
      

        public static T MakeCopy<T>(this T obj) where T: class
        {
            if (obj == null)
                return default(T);

            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }

        
    }
}
