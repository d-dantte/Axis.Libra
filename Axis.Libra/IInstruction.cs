using Axis.Libra.URI;

namespace Axis.Libra
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
        /// A unique signature representing this instruction, in the following format: <code>&lt;schema&gt;:&lt;namespace&gt;/&lt;property-hash&gt;</code>
        /// </summary>
        InstructionURI InstructionURI { get; }
    }
}
