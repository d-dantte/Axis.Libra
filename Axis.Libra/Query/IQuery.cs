using Axis.Libra.URI;

namespace Axis.Libra.Query
{
    /// <summary>
    /// A query. Represents instructions (parameters) that causes a system to retrieve data/information SYNCHRONIOUSLY. 
    /// </summary>
    public interface IQuery : IInstruction
    {
        /// <summary>
        /// A unique signature representing this query, in the following format: <code>qry:&lt;namespace&gt;/&lt;property hash&gt;</code>
        /// </summary>
        /// <returns></returns>
        InstructionURI QueryURI { get; }
    }
}
