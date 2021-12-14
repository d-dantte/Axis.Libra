using System;

namespace Axis.Libra.Exceptions
{
    public class RegistrationNotFoundException: Exception
    {
        public Type HandlerType { get; }

        public RegistrationNotFoundException(Type handlerType)
        : base($"Registration for handler '{handlerType}' was not found.")
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }
    }
}
