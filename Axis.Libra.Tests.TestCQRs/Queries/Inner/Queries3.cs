using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Queries.Inner
{
    [InstructionNamespace("axis:libra:test-crs:query3")]
    public class Query3 : AbstractQuery
    {
        public Guid Id { get; set; }
    }

    public class Query3Handler :
        IQueryHandler<Query3, Query3Result>,
        IQueryHandler<Query2, Query2Result>
    {
        public IOperation<Query3Result> ExecuteQuery(Query3 query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Query3)} handler executed.");
                return new Query3Result { QueryURI = query.QueryURI };
            });

        public IOperation<Query2Result> ExecuteQuery(Query2 query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Query2)} handler executed.");
                return new Query2Result { QueryURI = query.QueryURI };
            });
    }

    public class Query3Result : IQueryResult
    {
        public InstructionURI QueryURI { get; set; }
    }
}
