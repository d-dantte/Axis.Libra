using Axis.Libra.Request;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Requests
{
    [InstructionNamespace("axis:libra:test-crs:request1")]
    public class Request1: AbstractRequest
    {
        public int Age { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }
    }

    public class Request1Handler : IRequestHandler<Request1>
    {
        public IOperation<IResult> ExecuteRequest(Request1 request)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Request1)} handler executed.");
                return IResult.OfSuccess();
            });
    }
}
