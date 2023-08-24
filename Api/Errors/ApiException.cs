namespace Api.Errors
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
