using System.Runtime.Serialization;

namespace ApplicationServices.Interfaces.ChangeTracker
{
    [Serializable]
    public class InvalidChangeException : Exception, ISerializable
    {
        public InvalidChangeException()
        {

        }

        public InvalidChangeException(Exception ex) : base(ex.Message, ex)
        {

        }

        protected InvalidChangeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
          : base(serializationInfo, streamingContext)
        {
            //if (serializationInfo.GetValue("ExceptionSource", typeof(string)) is string exceptionSource)
            //{
            //    ExceptionSource = exceptionSource
            //}
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("ExceptionSource", ExceptionSource, typeof(string))
            base.GetObjectData(info, context);
        }
    }
}
