using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Commands
{
    [CommandStatus(typeof(Command1Status))]
    [InstructionNamespace("axis:libra:test-crs:command1")]
    public class Command1: AbstractCommand
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class Command1Status : ICommandStatusResult
    {
        public ICommandStatus? Status { get; set; }

        public InstructionURI CommandURI { get; set; }

        public InstructionURI QueryURI { get; set; }
    }

    public class Command1Handler :
        ICommandHandler<Command1>,
        IQueryHandler<CommandStatusQuery, Command1Status>
    {
        public IOperation<InstructionURI> ExecuteCommand(Command1 command)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Command1)} handler executed.");
                return command.CommandURI;
            });

        public IOperation<Command1Status> ExecuteQuery(CommandStatusQuery query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Command1Status)} handler executed.");
                return new Command1Status
                {
                    CommandURI = query.CommandURI,
                    QueryURI = query.QueryURI,
                    Status = new Random(Guid.NewGuid().GetHashCode()).Next(5) switch
                    {
                        0 => ICommandStatus.OfBusy(),
                        1 => ICommandStatus.OfSuccess(),
                        2 => ICommandStatus.OfError("something went wrong"),
                        3 => ICommandStatus.OfProgress(new Random(Guid.NewGuid().GetHashCode()).Next(101)),
                        4 => ICommandStatus.OfUnknown(),
                        _ => throw new Exception("Invalid status")
                    }
                };
            });
    }
}
