using Axis.Libra.Request;
using Axis.Libra.Instruction;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Requests.Inner
{
    [InstructionNamespace("axis:libra:test-crs:request3")]
    public class Request3 : AbstractRequest<Request3Result>
    {
        public Guid Id { get; set; }
    }

    public class Request3Handler :
        IRequestHandler<Request3, Request3Result>,
        IRequestHandler<Request2, Request2Result>
    {
        public async Task<IResult<Request3Result>> ExecuteRequest(Request3 request)
        {
            Console.WriteLine($"{typeof(Request1)} handler executed.");
            return IResult<Request3Result>.Of(new Request3Result
            {
                Meh = "bleh"
            });
        }

        public async Task<IResult<Request2Result>> ExecuteRequest(Request2 request)
        {
            Console.WriteLine($"{typeof(Request1)} handler executed.");
            return IResult<Request2Result>.Of(new Request2Result
            {
                TimeStamp = DateTimeOffset.Now
            });
        }
    }

    public class Request3Result
    {
        public string? Meh { get; set; }
    }
}
