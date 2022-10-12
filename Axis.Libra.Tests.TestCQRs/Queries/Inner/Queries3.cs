using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Queries.Inner
{
    [InstructionNamespace("axis:libra:test-crs:query3")]
    public class Query3 : AbstractQuery<Query3Result>
    {
        public Guid Id { get; set; }
    }

    public class Query3Handler :
        IQueryHandler<Query3, Query3Result>,
        IQueryHandler<Query2, Query2Result>
    {
        public async Task<IResult<Query3Result>> ExecuteQuery(Query3 query)
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            return IResult<Query3Result>.Of(new Query3Result
            {
                Meh = "bleh"
            });
        }

        public async Task<IResult<Query2Result>> ExecuteQuery(Query2 query)
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            return IResult<Query2Result>.Of(new Query2Result
            {
                TimeStamp = DateTimeOffset.Now
            });
        }
    }

    public class Query3Result
    {
        public string? Meh { get; set; }
    }
}
