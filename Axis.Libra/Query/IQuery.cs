namespace Axis.Libra.Query
{
    /// <summary>
    /// A query
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// A unique signature representing this query, typically built by getting a hash of the Query's properties.
        /// </summary>
        /// <returns></returns>
        string QuerySignature { get; }
    }
}
