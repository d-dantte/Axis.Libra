using Axis.Libra.URI;

namespace Axis.Libra.Query
{
    /// <summary>
    /// Represens the result of a specific query.
    /// </summary>
    public interface IQueryResult
    {
        /// <summary>
        /// Signature of the query that generated this result
        /// </summary>
        InstructionURI QueryURI { get; }
    }
}
