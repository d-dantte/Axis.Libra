using System;

namespace Axis.Libra.Exceptions
{
    /// <summary>
    /// Raised when a handler type cannot be resolved by the <see cref="Proteus.IoC.IResolverContract"/>
    /// </summary>
    public class UnresolvedHandlerException: Exception
    {
        /// <summary>
        /// The unresolved handler type
        /// </summary>
        public Type HandlerType { get; }

        public UnresolvedHandlerException(
            Type handlerType)
            :base($"Invalid resolution: the handler type '{handlerType}' could not be resolved")
        {
            ArgumentNullException.ThrowIfNull(handlerType);

            HandlerType = handlerType;
        }
    }
}
