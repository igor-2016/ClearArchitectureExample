using ApplicationServices.Interfaces.Extensions;
using Collecting.Controllers.Responses;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Utils;

namespace Collecting.Controllers.Filters.Exceptions
{

    /// <summary>
    /// Обработчик ошибок метода changeCollecting CollectController
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CollectingExceptionHandlerAttribute : Attribute, IAsyncExceptionFilter
    {
        private readonly string _contentType = "application/xml";

        private ILogger? _logger;

        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger = _logger ?? 
                context.HttpContext.RequestServices.GetService(typeof(ILogger<CollectingExceptionHandlerAttribute>)) as ILogger;
            _logger?.LogCustomError(context.Exception, context.HttpContext);

            if (context.Exception is DescribedException describedException)
            {
                context.Result = new ContentResult
                {
                    Content = ConfirmResponse
                        .Error(describedException.ErrorCode, describedException.ErrorMessage).ToXmlUtf8(),
                    StatusCode = (int)describedException.HttpErrorCode,
                    ContentType = _contentType,
                };
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = ConfirmResponse
                        .Error(StatusCodes.Status501NotImplemented, 
                            context.Exception.GetBaseException().Message).ToXmlUtf8(),
                    StatusCode = StatusCodes.Status501NotImplemented,
                    ContentType = _contentType,
                };
            }

            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
