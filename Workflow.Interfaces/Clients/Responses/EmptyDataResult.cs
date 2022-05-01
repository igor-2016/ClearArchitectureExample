using System.Net;
using Workflow.Interfaces.Exceptions;

namespace Workflow.Interfaces.Clients.Responses
{
    public class EmptyDataResult
    {
        public bool IsSuccess { get; }

        public bool HasError { get => AnError != null; }

        public Exception AnError { get; }

       


        public HttpStatusCode? StatusCode { get; }

        protected EmptyDataResult(bool isSuccess, Exception error, HttpStatusCode? statusCode)
        {
            IsSuccess = isSuccess;
            AnError = error;
            StatusCode = statusCode;

        }

        public static EmptyDataResult Success(HttpStatusCode statusCode) =>
            new EmptyDataResult(true, null, statusCode);


        public static EmptyDataResult Error(WorkflowErrors error, Exception ex) =>
            new EmptyDataResult(false, new WorkflowException(error, ex), null);

        public static EmptyDataResult Error(WorkflowErrors error, string errorMessage, HttpStatusCode statusCode, Exception ex) =>
            new EmptyDataResult(false, new WorkflowException(error, errorMessage, statusCode, ex), statusCode);

        public static EmptyDataResult Error(WorkflowErrors error, string errorMessage, HttpStatusCode statusCode) =>
            new EmptyDataResult(false, new WorkflowException(error, errorMessage, statusCode), statusCode);

        public static EmptyDataResult NotFound(WorkflowErrors error, string errorMessage) =>
             new EmptyDataResult(false, new WorkflowException(error, errorMessage, HttpStatusCode.NotFound), HttpStatusCode.NotFound);

    }
   
}
