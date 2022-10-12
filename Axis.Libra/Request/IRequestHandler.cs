using Axis.Luna.Common;
using Axis.Luna.Operation;
using System.Threading.Tasks;

namespace Axis.Libra.Request
{
    /// <summary>
    /// Implementations contain logic that process a specific <see cref="IRequest"/> instance.
    /// </summary>
    public interface IRequestHandler<in TRequest, TResult>
    where TRequest : IRequest<TResult>
    {
        /// <summary>
        /// Executes the request instruction and returns an <see cref="Operation"/> instance that encapsulates the outcome of that operation.
        /// </summary>
        /// <param name="request">The request instance</param>
        Task<IResult<TResult>> ExecuteRequest(TRequest request);
    }
}
