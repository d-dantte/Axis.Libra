using Axis.Libra.Exceptions;
using Axis.Luna.Common;
using Axis.Proteus.IoC;
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
        /// <param name="command"></param>
        /// <returns>An <see cref="Operation{TResult}"/> encapsulating the command signature used to query for it's results</returns>
        public Task<IResult<TResult>> Dispatch<TQuery, TResult>(TQuery command)
        where TQuery : IQuery<TResult>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return _manifest
                .HandlerFor<TQuery, TResult>()
                ?.ExecuteQuery(command)
                ?? throw new InvalidOperationException($"could not find a handler for the command of type: {typeof(TQuery)}");
        }
    }
}
