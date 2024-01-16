using System;

namespace Axis.Libra.Exceptions
{
    /// <summary>
    /// Raised when no handler is found for a given instruction within the instruction manifest
    /// </summary>
    public class UnregisteredInstructionException: Exception
    {
        /// <summary>
        /// The instruction for which a registration within the given manifest could not be found
        /// </summary>
        public object Instruction { get; }

        public UnregisteredInstructionException(
            object instruction)
            :base($"Invalid registration: no handler was found for the instruction")
        {
            ArgumentNullException.ThrowIfNull(instruction);

            Instruction = instruction;
        }
    }
}
