using Axis.Luna.Operation;

namespace Axis.Libra.Query
{
    /// <summary>
    /// The Query Hander interface. The ideal use-case scenario for this interface is to have concrete types that implement
    /// the interface, but differ only in the combination of generic arguments given to the interface. This means one MUST
    /// NOT create another interface that implements this one.
    /// <para>
    /// See also: <see cref="QueryDispatcher.HandlerFor{TQuery, TQueryResult}"/>
    /// </para>
    /// </summary>
    /// <typeparam name="TQuery">The Query type</typeparam>
    /// <typeparam name="TQueryResult">The Query Result type</typeparam>
    public interface IQueryHandler<in TQuery, TQueryResult>
    where TQuery: IQuery
    where TQueryResult: IQueryResult
    {
        IOperation<TQueryResult> ExecuteQuery(TQuery query);
    }
}
