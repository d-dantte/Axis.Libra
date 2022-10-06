using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Commands.Inner
{
    [CommandStatus(typeof(Command3Status))]
    [InstructionNamespace("axis:libra:test-cqrs:command3")]
    public class Command3 : AbstractCommand
    {
        public Guid Id { get; set; }
    }

    public class Command3Status : ICommandStatusResult
    {
        public ICommandStatus? Status { get; set; }

        public InstructionURI CommandURI { get; set; }

        public InstructionURI QueryURI { get; set; }
    }

    public class Command3Handler :
        ICommandHandler<Command3>,
        ICommandHandler<Command2>,
        IQueryHandler<CommandStatusQuery, Command3Status>,
        IQueryHandler<CommandStatusQuery, Command2Status>
    {
        public IOperation<InstructionURI> ExecuteCommand(Command3 command)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Command3)} handler executed.");
                return command.CommandURI;
            });

        public IOperation<InstructionURI> ExecuteCommand(Command2 command)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Command2)} handler executed.");
                return command.CommandURI;
            });

        public IOperation<Command3Status> ExecuteQuery(CommandStatusQuery query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Command3Status)} handler executed.");
                return new Command3Status
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

        IOperation<Command2Status> IQueryHandler<CommandStatusQuery, Command2Status>.ExecuteQuery(CommandStatusQuery query)
            => Operation.Try(() =>
            {
                Console.WriteLine($"{typeof(Command2Status)} handler executed.");
                return new Command2Status
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
