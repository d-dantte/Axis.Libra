using Axis.Libra.Request;
using Axis.Libra.URI;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Requests
{
    [InstructionNamespace("axis:libra:test-crs:request2")]
    public class Request2 : AbstractRequest<Request2Result>
    {
        public Uri? Something { get; set; }
    }

    public class Request2Result
    {
        public DateTimeOffset TimeStamp { get; set; }
    }

    public class Request2Handler : IRequestHandler<Request2, Request2Result>
    {
        public async Task<IResult<Request2Result>> ExecuteRequest(Request2 request)
        {
            Console.WriteLine($"{typeof(Request2)} handler executed.");
            return IResult<Request2Result>.Of(new Request2Result
            {
                TimeStamp = DateTimeOffset.Now
            });
        }
    }
}
