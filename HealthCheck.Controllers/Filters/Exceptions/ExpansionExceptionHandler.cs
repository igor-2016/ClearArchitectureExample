using ApplicationServices.Interfaces.Extensions;
using ECom.Types.ServiceBus;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Utils;

namespace HealthCheck.Controllers.Filters.Exceptions
{

    /// <summary>
    /// Обработчик ошибок метода  WorkflowController
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExpansionExceptionHandlerAttribute : Attribute, IAsyncExceptionFilter
    {
        private readonly string _contentType = "application/json";
        private  ILogger? _logger;

     

        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger = _logger ?? 
                context.HttpContext.RequestServices.GetService(typeof(ILogger<ExpansionExceptionHandlerAttribute>)) as ILogger;
            _logger?.LogCustomError(context.Exception, context.HttpContext);

            if (context.Exception is DescribedException describedException)
            {
                context.Result = new ContentResult
                {
                    Content = describedException.GetEComExceptionResponse(EComErrorType.Warning).ToJson(),
                    StatusCode = (int)describedException.HttpErrorCode,
                    ContentType = _contentType,
                };
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = context.Exception.GetEComExceptionResponse(EComErrorType.Error).ToJson(),
                    StatusCode = StatusCodes.Status501NotImplemented,
                    ContentType = _contentType,
                };
            }

            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
