using Axis.Libra.Command;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.LocalTypes
{
    [CommandStatus(typeof(LocalCommandStatus))]
    [InstructionNamespace("axis:libra:tests:local-types:LocalCommand")]
    public class LocalCommand: AbstractCommand
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }

    public class LocalCommandStatus : ICommandStatusResult
    {
        public ICommandStatus Status { get; set; }

        public InstructionURI CommandURI { get; set; }

        public InstructionURI QueryURI { get; set; }
    }

    public class LocalCommandHandler : ICommandHandler<LocalCommand>
    {
        public IOperation<InstructionURI> ExecuteCommand(LocalCommand command)
            => Operation.Try(() => command.CommandURI);
    }
}
