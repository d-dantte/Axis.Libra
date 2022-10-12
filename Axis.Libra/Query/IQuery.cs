using Axis.Libra.URI;

namespace Axis.Libra.Query
{
    /// <summary>
    /// A query. Represents instructions (parameters) that causes a system to retrieve data/information SYNCHRONIOUSLY. 
    /// </summary>
    public interface IQuery<TResult> : IInstruction
    {
    }
}
