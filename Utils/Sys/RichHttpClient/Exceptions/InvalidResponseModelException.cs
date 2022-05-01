namespace Utils.Sys.Exceptions
{
    public class InvalidResponseModelException : Exception
    {
        public string? UnknownResponse { get; set; }

        public InvalidResponseModelException(string? unknownResponse) : base("Неизвестный ответ: " + unknownResponse)
        {
            UnknownResponse = unknownResponse;
        }
    }

}
