using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.URI;
using Axis.Luna.Common;
using Axis.Luna.Operation;

namespace Axis.Libra.Tests.TestCQRs.Commands.Inner
{
    [InstructionNamespace("axis:libra:test-cqrs:command3")]
    public class Command3 : AbstractCommand
    {
        public Guid Id { get; set; }
    }

    public class Command3Handler :
        ICommandHandler<Command3>,
        ICommandHandler<Command2>
    {
        public async Task<IResult<ICommandStatus>> ExecuteSatusRequest(InstructionURI commandURI)
        {
            var status = new Random(Guid.NewGuid().GetHashCode()).Next(5) switch
            {
                0 => ICommandStatus.OfBusy(commandURI),
                1 => ICommandStatus.OfSuccess(commandURI),
                2 => ICommandStatus.OfError(commandURI, "something went wrong"),
                3 => ICommandStatus.OfProgress(commandURI, new Random(Guid.NewGuid().GetHashCode()).Next(101)),
                4 => ICommandStatus.OfUnknown(commandURI),
                _ => throw new Exception("Invalid status")
            };

            Console.WriteLine($"status request for: {commandURI}");
            return IResult<ICommandStatus>.Of(status);
        }

        public async Task<IResult<InstructionURI>> ExecuteCommand(Command2 command)
        {
            Console.WriteLine($"{typeof(Command1)} handler executed.");
            return IResult<InstructionURI>.Of(command.InstructionURI);
        }

        public async Task<IResult<InstructionURI>> ExecuteCommand(Command3 command)
        {
            Console.WriteLine($"{typeof(Command1)} handler executed.");
            return IResult<InstructionURI>.Of(command.InstructionURI);
        }
    }
}
