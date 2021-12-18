using Axis.Luna.Operation;

namespace Axis.Libra.Query
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    public interface IQueryHandler<in TQuery, TQueryResult>
    where TQuery: IQuery
    where TQueryResult: IQueryResult
    {
        Operation<TQueryResult> ExecuteQuery(TQuery query);
    }
}
