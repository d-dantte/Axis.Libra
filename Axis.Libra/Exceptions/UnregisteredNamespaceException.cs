using Axis.Libra.Instruction;
using System;

namespace Axis.Libra.Exceptions
{
    /// <summary>
    /// Raised when no handler is found for a given namespace within the instruction manifest
    /// </summary>
    public class UnregisteredNamespaceException: Exception
    {
        /// <summary>
        /// The namespace for which a registration within the given manifest could not be found
        /// </summary>
        public InstructionNamespace Namespace { get; }

        public UnregisteredNamespaceException(
            InstructionNamespace @namespace)
            :base($"Invalid registration: no handler was found for the namespace '{@namespace}'")
        {
            Namespace = @namespace;
        }
    }
}
