using Axis.Luna.Common.Results;
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
        /// <param name="request"></param>
        public Task<IResult<TResult>> Dispatch<TRequest, TResult>(TRequest request)
        where TRequest : IRequest<TResult>
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return _manifest
                .HandlerFor<TRequest, TResult>()
                ?.ExecuteRequest(request)
                ?? throw new InvalidOperationException(
                    $"Invalid {nameof(request)}: No handler found for '{typeof(TRequest)}'");
        }
    }
}
