using Axis.Libra.Exceptions;
using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Axis.Libra.Query
{
    public class QueryDispatcher
    {
        private static readonly MethodInfo _GenericDipatchMethod = typeof(QueryDispatcher)
            .GetMethods()
            .Where(method => method.IsPublic)
            .Where(method => !method.IsStatic)
            .Where(method => method.Name.Equals(nameof(Dispatch)))
            .Where(method => method.IsGenericMethodDefinition)
            .Where(method => method.GetGenericArguments().Length == 2)
            .Where(method => method.GetParameters().Length == 1)
            .FirstOrDefault();

        // This cache is unnecessary since Axis.FInvoke already caches the invokers using the method
        // instance as the key
        private static readonly ConcurrentDictionary<(Type queryType, Type resultType), InstanceInvoker> _InvokerCache = new ConcurrentDictionary<(Type, Type), InstanceInvoker>();


        private readonly IResolverContract _serviceResolver;

        public QueryDispatcher(IResolverContract serviceResolver)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        /// <summary>
        /// Dispatches the query to a registered handler
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IOperation<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query)
        where TQuery : IQuery
        where TQueryResult : IQueryResult
        => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return this
                .HandlerFor<TQuery, TQueryResult>()
                ?.ExecuteQuery(query)
                ?? throw new UnknownResolverException(typeof(IQueryHandler<TQuery, TQueryResult>));
        });

        /// <summary>
        /// Dispatches the query for a commands status to a registered handler
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandManifest"></param>
        /// <returns></returns>
        public IOperation<Command.ICommandStatusResult> Dispatch(
           Command.CommandStatusQuery query,
           Command.CommandManifest commandManifest)
        {
            var resultType = commandManifest.StatusResultFor(query.CommandURI.Namespace);
            var invoker = _InvokerCache.GetOrAdd((typeof(Command.CommandStatusQuery), resultType), tuple =>
            {
                return _GenericDipatchMethod
                    .MakeGenericMethod(tuple.queryType, tuple.resultType)
                    .Map(InstanceInvoker.InvokerFor);
            });

            return invoker.Func
                .Invoke(this, new object[] { query })
                .As<IOperation<Command.ICommandStatusResult>>();
        }

        private IQueryHandler<TQuery, TQueryResult> HandlerFor<TQuery, TQueryResult>()
        where TQuery : IQuery
        where TQueryResult : IQueryResult
        => _serviceResolver.Resolve<IQueryHandler<TQuery, TQueryResult>>();
    }
}
