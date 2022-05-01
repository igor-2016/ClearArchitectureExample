namespace Utils.Sys.Exceptions
{
    public class ReadResponseException : Exception
    {
        public ReadResponseException(Exception ex): base(ex.Message, ex)
        {
        }
    }
}
