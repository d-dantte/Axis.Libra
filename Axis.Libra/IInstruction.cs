using Axis.Libra.URI;

namespace Axis.Libra
{
    /// <summary>
    /// Root interface for requests, commands, and queries.
    /// </summary>
    public interface IInstruction
    {
        /// <summary>
        /// A unique signature representing this query, in the following format: <code>qry:&lt;namespace&gt;/&lt;property hash&gt;</code>
        /// </summary>
        InstructionURI InstructionURI { get; }trge
    }
}
