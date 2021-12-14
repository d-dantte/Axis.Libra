using Axis.Luna.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Libra.Query
{
    public class QueryDispatcher
    {
        private readonly Dictionary<Type, (Type, object)> _handlers = new Dictionary<Type, (Type, object)>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool RegisterHandler<TQuery, TResult>(IQueryHandler<TQuery, TResult> handler)
        where TQuery: IQuery
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_handlers.ContainsKey(typeof(TQuery)))
                return false;

            _handlers.Add(typeof(TQuery), (typeof(TResult), handler));
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int HandlerCount() => _handlers.Count;

        /// <summary>
        /// Returns an enumerable of a tuple containing the <c>TQuery</c>, and the corresponding <c>TResult</c>, in that order.
        /// </summary>
        public IEnumerable<(Type, Type)> RegisteredQueries() => _handlers.Select(kvp => (kvp.Key, kvp.Value.Item1));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Operation<IQueryResult<TResult>> Dispatch<TQuery, TResult>(TQuery query)
        where TQuery : IQuery => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var handler = HandlerFor<TQuery, TResult>();

            /// At this point, we can call into filter logic that'll support AOP-like cross-cutting features like logging, etc...

            return handler.ExecuteQuery(query);
        });


        private bool IsQueryTypeRegistered(Type queryType, Type queryResultType)
        {
            return _handlers.ContainsKey(queryType)
                && queryResultType.Equals(_handlers[queryType].Item1);
        }

        private IQueryHandler<TQuery, TResult> HandlerFor<TQuery, TResult>()
        where TQuery : IQuery => IsQueryTypeRegistered(typeof(TQuery), typeof(TResult))
            ? _handlers[typeof(TQuery)] as IQueryHandler<TQuery, TResult>
            : throw new Exceptions.RegistrationNotFoundException(typeof(IQueryHandler<TQuery, TResult>));
    }
}
