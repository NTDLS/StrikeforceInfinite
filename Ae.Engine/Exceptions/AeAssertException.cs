namespace Ae.Engine.Exceptions
{
    public class AeAssertException
        : AeExceptionBase
    {
        public AeAssertException()
        {
        }

        public AeAssertException(string message)
            : base($"Assert exception: {message}.")
        {
        }
    }
}
