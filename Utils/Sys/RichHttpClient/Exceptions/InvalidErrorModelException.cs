namespace Utils.Sys.Exceptions
{
    public class InvalidErrorModelException: Exception
    {
        public string UnknownResponse { get; set; }

        public InvalidErrorModelException(string unknownResponse) : base("Неизвестный ответ: " + unknownResponse)
        {
            UnknownResponse = unknownResponse;
        }
    }
}
