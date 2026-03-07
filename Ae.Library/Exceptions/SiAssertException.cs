namespace Ae.Library.Exceptions
{
    public class SiAssertException : SiExceptionBase
    {
        public SiAssertException()
        {
        }

        public SiAssertException(string message)
            : base($"Assert exception: {message}.")
        {
        }
    }
}
