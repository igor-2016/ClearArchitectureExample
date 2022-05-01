namespace ECom.Expansion.Service.Utils
{
    public class RequestGuid
    {
        private readonly RequestDelegate _next;

        public RequestGuid(RequestDelegate next)
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
