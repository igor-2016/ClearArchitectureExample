using Prometheus;

namespace ECom.Expansion.Service.Utils
{
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Подключение общих обработчиков 
        ///   - RequestGuid
        ///   - Logging
        ///   - ExceptionHandling
        ///   - Prometheus metrics server
        ///   - Prometheus http metrics
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCommonMiddleware(this IApplicationBuilder builder)
        {

            // turn on prometheus exporter
            builder.UseMetricServer();

            // turn on default http_request metrics
            builder.UseHttpMetrics();

            builder.UseMiddleware<RequestGuid>();

            // TODO Exception Handling !!!

            //builder.UseMiddleware<Logging>();
            //builder.UseWhen(
            //    x => !x.Request.Path.StartsWithSegments("/ecom/klushacollect") &&
            //        !x.Request.Path.StartsWithSegments("/ecom/collect"),
            //    x => x.UseMiddleware<ExceptionHandling>()
            //);
            //builder.UseWhen(
            //    x => x.Request.Path.StartsWithSegments("/ecom/klushacollect"),
            //    x => x.UseMiddleware<KlushaExceptionHandling>()
            //);
            //builder.UseWhen(
            //    x => x.Request.Path.StartsWithSegments("/ecom/collect"),
            //    x => x.UseMiddleware<TSDExceptionHandling>()
            //);

            return builder;
        }
    }
}
