using Axis.Libra.Exceptions;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using System;

namespace Axis.Libra.Query
{
    public class QueryDispatcher
    {
        private readonly ServiceResolver _serviceResolver;

        public QueryDispatcher(ServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Operation<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query)
        where TQuery : IQuery
        where TQueryResult : IQueryResult
        => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return this
                .HandlerFor<TQuery, TQueryResult>()
                ?.ExecuteQuery(query)
                ?? throw new RegistrationNotFoundException(typeof(IQueryHandler<TQuery, TQueryResult>));
        });

        private IQueryHandler<TQuery, TQueryResult> HandlerFor<TQuery, TQueryResult>()
        where TQuery : IQuery
        where TQueryResult : IQueryResult
        => _serviceResolver.Resolve<IQueryHandler<TQuery, TQueryResult>>();
    }
}
