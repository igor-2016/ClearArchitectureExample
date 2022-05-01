using System.Runtime.Serialization;
using Utils;

namespace ApplicationServices.Interfaces.ChangeTracker
{

    [Serializable]
    public class NotifyChangeException : Exception, ISerializable
    {
        public object? SourceEntity { get; }
        public object? TargetEntity { get; }

       public ChangeType? ChangeType { get; }

        public NotifyChangeException()
        {

        }

        public NotifyChangeException(Exception ex): base(ex.Message, ex)
        {

        }

        public NotifyChangeException(ChangeType? changeType, object? source, object? target, Exception ex)
            : base($"Process entity error (source value: {source}, target value: {target})", ex)
        {
            ChangeType = changeType;
            SourceEntity = source;
            TargetEntity = target;
        }

       

        protected NotifyChangeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
          : base(serializationInfo, streamingContext)
        {
            //if (serializationInfo.GetValue("ExceptionSource", typeof(string)) is string exceptionSource)
            //
            //    ExceptionSource = exceptionSource
            //
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("ExceptionSource", ExceptionSource, typeof(string))
            base.GetObjectData(info, context);
        }
    }

}
