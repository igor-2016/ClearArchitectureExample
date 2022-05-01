using System.Runtime.Serialization;
using Utils.Consts;

namespace Utils.Exceptions
{
    /// <summary>
    /// Базовая ошибка.
    /// Содержит информацию о слое приложения, где произошла ошибка и сервисе (Экспансия) 
    /// </summary>
    [Serializable]
    public abstract class SourceException : ApplicationException, ISerializable
    {
        /// <summary>
        /// Модуль где произошла ошибка к Expansion
        /// </summary>
        public abstract string ExceptionSource { get; protected set; }

        /// <summary>
        /// Система, к которой обращается и она отдают ошибку. 
        /// По умолчанию наша система, так как никто ничего не проставил, если обращение было всё же в другую системы
        /// Используй AddRequestedTarget расширение для присвоения этого свойства
        /// </summary>
        public virtual string ExceptionTarget { get; set; } = CommonConsts.Subsystem.Unknown;

        public virtual string ServiceSource { get; protected set; } = CommonConsts.Project.ServiceSource;

        public override string? Source { get => $"{ServiceSource} : {ExceptionSource}"; }

        protected SourceException()
        {
        }

        protected SourceException(string message) : base(message)
        {
           
        }

        protected SourceException(string message, Exception innerException) : base(message, innerException)
        {
            
        }

        //TODO test on serialization
        protected SourceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
           : base(serializationInfo, streamingContext)
        {
            
            if (serializationInfo.GetValue("ExceptionSource", typeof(string)) is string exceptionSource)
            {
                ExceptionSource = exceptionSource;
            }

            if (serializationInfo.GetValue("ServiceSource", typeof(string)) is string serviceSource)
            {
                ServiceSource = serviceSource;
            }
            
            if (serializationInfo.GetValue("Source", typeof(string)) is string source)
            {
                Source = source;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ExceptionSource", ExceptionSource, typeof(string));
            info.AddValue("ServiceSource", ServiceSource, typeof(string));
            info.AddValue("Source", Source, typeof(string));
            base.GetObjectData(info, context);
        }

    }
}
