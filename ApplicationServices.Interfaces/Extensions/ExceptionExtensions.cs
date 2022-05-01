using ApplicationServices.Interfaces.Responses;
using ECom.Types.Responses;
using ECom.Types.ServiceBus;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Utils.Sys.RichHttpClient.Exceptions;

namespace ApplicationServices.Interfaces.Extensions
{
    public static class ExceptionExtensions
    {
        public static async Task HandleEComExceptionAsync(this HttpContext httpContext, HttpStatusCode httpErrorCode, 
            IDefaultEComResponse defaultEComResponse)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)httpErrorCode;

            var stream = httpContext.Response.Body;
            await JsonSerializer.SerializeAsync(stream, defaultEComResponse);
        }

        public static IDefaultEComResponse GetEComExceptionResponse(this Exception ex, EComErrorType comErrorType )
        {
            IDefaultEComResponse response;
            if (ex is DescribedException describedException)
            {
                response = new DefaultEComResponse()
                {
                    EComError = new EComError()
                    {
                        ErrorCode = describedException.ErrorCode,
                        ErrorId = describedException.ErrorId,
                        ErrorMessage = describedException.ErrorMessage,
                        ErrorDescription = $"{describedException.ErrorDescription} ({describedException.ExceptionSource}, {describedException.ExceptionTarget})",
                        ErrorType = comErrorType
                    }
                };
            }
            else if(ex is RichHttpClientException clientException)
            {
                response = new DefaultEComResponse()
                {
                    EComError = new EComError()
                    {
                        ErrorCode = (int)(clientException?.Code ?? HttpStatusCode.InternalServerError),
                        ErrorId = Guid.NewGuid().ToString(),
                        ErrorMessage = ex.Message,
                        ErrorDescription = clientException?.Response ?? "",
                        ErrorType = comErrorType
                    }
                };
            }
            else
            {
                response = new DefaultEComResponse()
                {
                    EComError = new EComError()
                    {
                        ErrorCode = 501,
                        ErrorId = Guid.NewGuid().ToString(),
                        ErrorMessage = ex.Message,
                        ErrorDescription = "Необработанная ошибка!",
                        ErrorType = comErrorType
                    }
                };
            }
            return response;
        }
    }
}
