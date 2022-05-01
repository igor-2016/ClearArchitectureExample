namespace Collecting.Interfaces.Exceptions
{
    public interface IErrorCodeResponse  
    {
        int errorCode { get; set; }
        string errorMessage { get; set; }
    }
}
