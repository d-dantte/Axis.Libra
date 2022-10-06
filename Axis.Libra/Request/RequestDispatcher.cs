using Axis.Libra.Exceptions;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using System;

namespace Axis.Libra.Request
{
    internal class RequestDispatcher
    {
        private readonly IResolverContract _serviceResolver;

        public RequestDispatcher(IResolverContract serviceResolver)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        /// <summary>
        /// Dispatches the command to be handled immediately by the registered handler
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns>An <see cref="Operation{TResult}"/> encapsulating the command signature used to query for it's results</returns>
        public IOperation<IResult> Dispatch<TRequest>(TRequest request)
        where TRequest : IRequest => Operation.Try(() =>
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return this
                .HandlerFor<TRequest>()
                ?.ExecuteRequest(request)
                ?? throw new UnknownResolverException(typeof(IRequestHandler<TRequest>));
        });

        private IRequestHandler<TRequest> HandlerFor<TRequest>()
        where TRequest : IRequest => _serviceResolver.Resolve<IRequestHandler<TRequest>>();
    }
}
