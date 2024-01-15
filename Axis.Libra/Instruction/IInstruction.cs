using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.Request;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Instruction
{
    /// <summary>
    /// Root interface for requests, commands, and queries.
    /// <para>
    /// All concrete implementations of this interface must be decorated with the <see cref="InstructionNamespaceAttribute"/>.
    /// </para>
    /// </summary>
    public interface IInstruction
    {
        /// <summary>
        /// Returns a unique hash for the instruction. Ideally, this is a hash of the internal state of the instruction,
        /// and should be guaranteed to be "unique" across all instances of the same instruction type.
        /// </summary>
        /// <returns>A hash value of the internal state of the instruction</returns>
        InstructionHash InstructionHash();

        /// <summary>
        /// Returns a namespace for the instruction. Being static attempts to enforce the same value for all instances
        /// of the same type
        /// </summary>
        /// <returns>A namespace-name for the instruction</returns>
        public static abstract InstructionNamespace InstructionNamespace();
    }


    public static class InstructionExtension
    {
        /// <summary>
        /// A unique signature representing this instruction, in the following format: <code>&lt;scheme&gt;:&lt;namespace&gt;/&lt;property-hash&gt;</code>
        /// </summary>
        public static InstructionURI InstructionURI(
            this IInstruction instruction)
        {
            return new(
                instruction.InstructionScheme(),
                instruction.InstructionNamespace(),
                instruction.InstructionHash());
        }

        public static Scheme InstructionScheme(this IInstruction instruction)
        {
            ArgumentNullException.ThrowIfNull(instruction);

            var itype = instruction.GetType();
            var rtype = typeof(IRequest<>);
            var qtype = typeof(IQuery<>);
            return instruction switch
            {
                ICommand => Scheme.Command,
                _ when itype.ImplementsGenericInterface(qtype) => Scheme.Query,
                _ when itype.ImplementsGenericInterface(rtype) => Scheme.Request,
                _ => throw new InvalidOperationException(
                    $"Invalid instruction type: {itype?.ToString() ?? "null"}")
            };
        }
    }
}
