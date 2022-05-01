using Entities;
using System.Net;
using System.Runtime.Serialization;
using Utils.Attributes;
using Utils.Consts;

namespace Workflow.Interfaces.Exceptions
{
    [Serializable]
    public class WorkflowException : DescribedException
    {
        public override string ExceptionSource { get; protected set; } = CommonConsts.Project.WorkflowService;

        public WorkflowException(WorkflowErrors error, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), ex)
        {
        }
        public WorkflowException(WorkflowErrors error, string description, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum(), ex)
        {
        }

        public WorkflowException(WorkflowErrors error, string description, HttpStatusCode code) :
            base((int)error, error.GetResponseMessageFromEnum(), description, code)
        {
        }

        public WorkflowException(WorkflowErrors error, string description, HttpStatusCode code, Exception ex) :
            base((int)error, error.GetResponseMessageFromEnum(), description, code, ex)
        {
        }

        public WorkflowException(WorkflowErrors error, string description) :
            base((int)error, error.GetResponseMessageFromEnum(), description, error.GetHttpStatusFromEnum())
        {
        }

        public WorkflowException(WorkflowErrors error) :
            base((int)error, error.GetResponseMessageFromEnum(), error.GetHttpStatusFromEnum())
        {
        }

        protected WorkflowException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {

        }
    }
}
