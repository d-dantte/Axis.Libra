using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Queries
{
    [InstructionNamespace("axis:libra:test-crs:query1")]
    public class Query1: AbstractQuery<Query1Result>
    {
        public int Age { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }
    }

    public class Query1Result
    {
        public int Stuff { get; set; }
    }

    public class Query1Handler : IQueryHandler<Query1, Query1Result>
    {
        public async Task<IResult<Query1Result>> ExecuteQuery(Query1 query)
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            return IResult<Query1Result>.Of(new Query1Result
            {
                Stuff = 34
            });
        }
    }
}
