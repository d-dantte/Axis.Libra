using Axis.Luna.Common.Results;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryDispatcher
    {
        private readonly QueryManifest _manifest;

        public QueryDispatcher(QueryManifest manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        /// <summary>
        /// Dispatches the command to be handled immediately by the registered handler
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        public Task<IResult<TResult>> Dispatch<TQuery, TResult>(TQuery query)
        where TQuery : IQuery<TResult>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return _manifest
                .HandlerFor<TQuery, TResult>()
                ?.ExecuteQuery(query)
                ?? throw new InvalidOperationException(
                    $"Invalid {nameof(query)}: No handler found for '{typeof(TQuery)}'");
        }
    }
}
