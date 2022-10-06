using Axis.Luna.Operation;

namespace Axis.Libra.Request
{
    /// <summary>
    /// Implementations contain logic that process a specific <see cref="IRequest"/> instance.
    /// </summary>
    public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
    {
        /// <summary>
        /// Executes the request instruction and returns an <see cref="Operation"/> instance that encapsulates the outcome of that operation.
        /// </summary>
        /// <param name="request">The request instance</param>
        IOperation<IResult> ExecuteRequest(TRequest request);
    }
}
