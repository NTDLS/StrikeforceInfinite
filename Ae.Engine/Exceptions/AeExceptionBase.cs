using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Exceptions
{
    public class AeExceptionBase
        : Exception
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
