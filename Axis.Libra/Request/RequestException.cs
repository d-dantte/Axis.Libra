using System;

namespace Axis.Libra.Request
{
    public class RequestException: Exception
    {
        public string Code { get; }

        public RequestException(string message, string code = null, Exception cause = null)
        : base(message, cause)
        {
            Code = code;
        }
    }
}
