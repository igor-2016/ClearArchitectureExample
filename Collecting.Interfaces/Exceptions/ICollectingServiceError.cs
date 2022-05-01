using Collecting.Interfaces.Exceptions;

namespace Collecting.Interfaces
{
    public interface ICollectingServiceError
    {
        IErrorCodeResponse ResponseMessage();
    }
}
