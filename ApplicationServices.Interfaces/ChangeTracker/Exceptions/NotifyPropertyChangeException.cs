using System.Runtime.Serialization;

namespace ApplicationServices.Interfaces.ChangeTracker
{
    [Serializable]
    public class NotifyPropertyChangeException : Exception, ISerializable
    {
    
        public string? PropertyName { get; }
        protected object? SourceValue { get; }
        protected object? TargetValue { get; }

        public NotifyPropertyChangeException()
        {

        }

        public NotifyPropertyChangeException(string? propertyName, object? sourceValue, object? targetValue, Exception ex) 
            : base($"Process property {propertyName} error (source value: {sourceValue}, target value: {targetValue})",  ex)
        {
            PropertyName = propertyName;
            SourceValue = sourceValue;
            TargetValue = targetValue;
        }

        protected NotifyPropertyChangeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
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
