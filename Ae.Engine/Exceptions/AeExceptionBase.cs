using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Exceptions
{
    public class AeExceptionBase
        : Exception
    {
        public AeLogSeverity Severity { get; set; }

        public AeExceptionBase()
        {
            Severity = AeLogSeverity.Exception;
        }

        public AeExceptionBase(string? message)
            : base(message)

        {
            Severity = AeLogSeverity.Exception;
        }
    }
}
