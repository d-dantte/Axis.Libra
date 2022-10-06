using Axis.Libra.Request;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Requests.Inner
{
    [InstructionNamespace("axis:libra:test-crs:query3")]
    public class Request3 : AbstractRequest
    {
        public Guid Id { get; set; }
    }

    public class Request3Handler :
        IRequestHandler<Request3>,
        IRequestHandler<Request2>
    {
        public IOperation<IResult> ExecuteRequest(Request3 query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Request3)} handler executed.");
                return IResult.OfSuccess();
            });

        public IOperation<IResult> ExecuteRequest(Request2 query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Request2)} handler executed.");
                return IResult.Of(Guid.NewGuid());
            });
    }
}
