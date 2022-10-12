using Axis.Libra.Exceptions;
using Axis.Luna.Common;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestDispatcher
    {
        private readonly RequestManifest _manifest;

        public RequestDispatcher(RequestManifest manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        /// <summary>
        /// Dispatches the command to be handled immediately by the registered handler
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns>An <see cref="Operation{TResult}"/> encapsulating the command signature used to query for it's results</returns>
        public Task<IResult<TResult>> Dispatch<TRequest, TResult>(TRequest command)
        where TRequest : IRequest<TResult>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return _manifest
                .HandlerFor<TRequest, TResult>()
                ?.ExecuteRequest(command)
                ?? throw new UnknownResolverException(typeof(IRequestHandler<TRequest, TResult>));
        }
    }
}
