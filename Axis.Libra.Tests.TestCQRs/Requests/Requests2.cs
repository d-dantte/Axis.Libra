using Axis.Libra.Request;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Requests
{
    [InstructionNamespace("axis:libra:test-crs:request2")]
    public class Request2 : AbstractRequest
    {
        public decimal Price { get; set; }
    }

    public class Request2Handler : IRequestHandler<Request2>
    {
        public IOperation<IResult> ExecuteRequest(Request2 request)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Request2)} handler executed.");
                return IResult.OfSuccess();
            });
    }
}
