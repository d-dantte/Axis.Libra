using System;

namespace Axis.Libra.Exceptions
{
    public class UnknownResolverException: Exception
    {
        public Type HandlerType { get; }

        public UnknownResolverException(Type handlerType)
        : base($"Registration for handler '{handlerType}' yielded null.")
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }
    }
}
