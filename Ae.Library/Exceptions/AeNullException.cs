namespace Ae.Library.Exceptions
{
    public class AeNullException : AeExceptionBase
    {
        public AeNullException()
        {
        }

        public AeNullException(string message)
            : base($"Null exception: {message}.")
        {
        }
    }
}