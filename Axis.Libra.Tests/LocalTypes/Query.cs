using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.LocalTypes
{
    [InstructionNamespace("axis:libra:tests:local-types:LocalQuery")]
    public class LocalQuery : AbstractQuery
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }

    public class LocalQueryResult : IQueryResult
    {
        public InstructionURI QueryURI { get; set; }
    }

    public class LocalQueryHandler : IQueryHandler<LocalQuery, LocalQueryResult>
    {
        public IOperation<LocalQueryResult> ExecuteQuery(LocalQuery query)
            => Operation.Try(() => new LocalQueryResult
            {
                QueryURI = query.QueryURI
            });
    }
}
