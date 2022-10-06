using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Queries
{
    [InstructionNamespace("axis:libra:test-crs:query1")]
    public class Query1: AbstractQuery
    {
        public int Age { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }
    }

    public class Query1Result : IQueryResult
    {
        public InstructionURI QueryURI { get; set; }
    }

    public class Query1Handler : IQueryHandler<Query1, Query1Result>
    {
        public IOperation<Query1Result> ExecuteQuery(Query1 query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Query1)} handler executed.");
                return new Query1Result { QueryURI = query.QueryURI };
            });
    }
}
