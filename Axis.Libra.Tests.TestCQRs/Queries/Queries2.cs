using Axis.Libra.Query;
using Axis.Libra.Instruction;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Queries
{
    [InstructionNamespace("axis:libra:test-crs:query2")]
    public class Query2 : AbstractQuery<Query2Result>
    {
        public Uri? Something { get; set; }
    }

    public class Query2Result
    {
        public DateTimeOffset TimeStamp { get; set; }
    }

    public class Query2Handler : IQueryHandler<Query2, Query2Result>
    {
        public async Task<IResult<Query2Result>> ExecuteQuery(Query2 query)
        {
            Console.WriteLine($"{typeof(Query2)} handler executed.");
            return IResult<Query2Result>.Of(new Query2Result
            {
                TimeStamp = DateTimeOffset.Now
            });
        }
    }
}
