using System.Net;
using System.Runtime.Serialization;
using Utils;
using Utils.Attributes;
using Utils.Consts;
using Utils.Exceptions;

namespace Entities
{
   
    [Serializable]
    public class DescribedException : SourceException, ISerializable
    {
        /// <summary>
        /// Идентификатор ошибки для логирования
        /// </summary>
        public string ErrorId { get; set; } = Guid.NewGuid().ToString(); 
        public HttpStatusCode HttpErrorCode { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.Entities;

        public DescribedException()
        {

        }

        public DescribedException(int error, string message) : base(message)
        {
            ErrorCode = error;
            ErrorMessage = message;
        }

        public DescribedException(int error, string message, HttpStatusCode httpErrorCode) : base(message)
        {
            ErrorCode = error;
            ErrorMessage = message;
            HttpErrorCode = httpErrorCode;
        }

        public DescribedException(int error, string message, Exception ex) : base(message, ex)
        {
            ErrorCode = error;
            ErrorMessage = message;
        }

        public DescribedException(int error, string message, HttpStatusCode httpErrorCode, Exception ex) : base(message, ex)
        {
            ErrorCode = error;
            ErrorMessage = message;
            HttpErrorCode = httpErrorCode;
        }

        public DescribedException(int error, string message, string description) : base(message)
        {
            ErrorCode = error;
            ErrorMessage = message;
            ErrorDescription = description;
        }

        public DescribedException(int error, string message, string description, HttpStatusCode httpErrorCode) : base(message)
        {
            ErrorCode = error;
            ErrorMessage = message;
            ErrorDescription = description;
            HttpErrorCode = httpErrorCode;
        }

        public DescribedException(int error, string message, string description, Exception ex) : base(message, ex)
        {
            ErrorCode = error;
            ErrorMessage = message;
            ErrorDescription = description;
        }

        public DescribedException(int error, string message, string description, HttpStatusCode httpErrorCode, Exception ex) : 
            base(description, ex)
        {
            ErrorCode = error;
            ErrorMessage = message;
            ErrorDescription = description;
            HttpErrorCode = httpErrorCode;
        }

        public DescribedException(DescribedErrors error, string description, Exception ex) : base(description, ex)
        {
            ErrorCode = (int)error;
            ErrorMessage = error.GetResponseMessageFromEnum();
            ErrorDescription = description;
            HttpErrorCode = error.GetHttpStatusFromEnum();
        }

        public DescribedException(DescribedErrors error, Exception ex) : base(error.GetResponseMessageFromEnum(), ex)
        {
            ErrorCode = (int)error;
            ErrorMessage = Message;
            HttpErrorCode = error.GetHttpStatusFromEnum();
        }

        public DescribedException(DescribedErrors error) : base(error.GetResponseMessageFromEnum())
        {
            ErrorCode = (int)error;
            ErrorMessage = Message;
            HttpErrorCode = error.GetHttpStatusFromEnum();
        }

        protected DescribedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            if (serializationInfo.GetValue("ExceptionSource", typeof(string)) is string exceptionSource)
            {
                ExceptionSource = exceptionSource;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ExceptionSource", ExceptionSource, typeof(string));
            base.GetObjectData(info, context);
        }
    }
}
