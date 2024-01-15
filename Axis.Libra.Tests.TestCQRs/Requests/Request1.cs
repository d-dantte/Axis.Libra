using Axis.Libra.Request;
using Axis.Libra.Instruction;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Requests
{
    [InstructionNamespace("axis:libra:test-crs:request1")]
    public class Request1: AbstractRequest<Request1Result>
    {
        public int Age { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }
    }

    public class Request1Result
    {
        public int Stuff { get; set; }
    }

    public class Request1Handler : IRequestHandler<Request1, Request1Result>
    {
        public async Task<IResult<Request1Result>> ExecuteRequest(Request1 request)
        {
            Console.WriteLine($"{typeof(Request1)} handler executed.");
            return IResult<Request1Result>.Of(new Request1Result
            {
                Stuff = 34
            });
        }
    }
}
