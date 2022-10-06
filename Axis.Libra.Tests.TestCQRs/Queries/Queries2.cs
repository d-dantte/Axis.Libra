using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Queries
{
    [InstructionNamespace("axis:libra:test-crs:query2")]
    public class Query2 : AbstractQuery
    {
        public Uri? Something { get; set; }
    }

    public class Query2Result : IQueryResult
    {
        public InstructionURI QueryURI { get; set; }
    }

    public class Query2Handler : IQueryHandler<Query2, Query2Result>
    {
        public IOperation<Query2Result> ExecuteQuery(Query2 query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Query2)} handler executed.");
                return new Query2Result { QueryURI = query.QueryURI };
            });
    }
}
