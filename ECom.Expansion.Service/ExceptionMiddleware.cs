using ApplicationServices.Interfaces.Extensions;
using ECom.Types.Responses;
using ECom.Types.ServiceBus;
using Entities;
using System.Net;

namespace ECom.Expansion.WebApi
{
    /// <summary>
    /// Последний обработчик ошибок
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Последняя надежда обработать ошибку
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (DescribedException ex)
            {
                _logger.LogCustomError(ex, httpContext);

                await HandleExceptionAsync(httpContext, ex.HttpErrorCode, ex, EComErrorType.Warning);
            }
            catch (Exception ex)
            {
                _logger.LogCustomError(ex, httpContext);

                await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex, EComErrorType.Error);
            }
        }


        protected virtual async Task HandleExceptionAsync(HttpContext httpContext, HttpStatusCode httpStatusCode,
            Exception ex, EComErrorType errorType)
        {
            IDefaultEComResponse response;

            if (ex is DescribedException describedException)
            {
                response = describedException.GetEComExceptionResponse(EComErrorType.Warning);
            }
            else
            {
                response = ex.GetEComExceptionResponse(EComErrorType.Error);
            }

            await httpContext.HandleEComExceptionAsync(httpStatusCode, response);
        }
    }
}
