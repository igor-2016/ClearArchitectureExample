namespace Utils.Sys.Options
{
    public class HttpClientOptions
    {
        public string BaseUrl { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
