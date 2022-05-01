namespace ECom.Expansion.Service.Utils
{
    public class ServiceHeaders
    {
        private readonly RequestDelegate _next;

        public ServiceHeaders(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            Populate(httpContext);

            return _next(httpContext);
        }

        private void Populate(HttpContext httpContext)
        {
            var requestGuidName = "requestGuid";

            if (httpContext.Items.ContainsKey(requestGuidName))
                return;

            var requestGuid = Guid.NewGuid();
            httpContext.Items.Add(requestGuidName, requestGuid);
        }
    }
}
