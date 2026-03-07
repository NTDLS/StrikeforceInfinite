using static Ae.Library.AeConstants;

namespace Ae.Library.Exceptions
{
    public class AeExceptionBase : Exception
    {
        public SiLogSeverity Severity { get; set; }

        public AeExceptionBase()
        {
            Severity = SiLogSeverity.Exception;
        }

        public AeExceptionBase(string? message)
            : base(message)

        {
            Severity = SiLogSeverity.Exception;
        }
    }
}
