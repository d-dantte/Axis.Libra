using Axis.Libra.Request;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.LocalTypes
{
    [InstructionNamespace("axis:libra:tests:local-types:LocalRequest")]
    public class LocalRequest : AbstractRequest
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }

    public class LocalRequestHandler : IRequestHandler<LocalRequest>
    {
        public IOperation<IResult> ExecuteRequest(LocalRequest request)
            => Operation.Try(() => IResult.OfSuccess());
    }
}
