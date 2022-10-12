using Axis.Libra.Command;
using Axis.Libra.URI;
using Axis.Luna.Common;

namespace Axis.Libra.Tests.TestCQRs.Commands
{
    [InstructionNamespace("axis:libra:test-crs:command1")]
    public class Command1: AbstractCommand
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class Command1Handler :
        ICommandHandler<Command1>
    {
        public async Task<IResult<ICommandStatus>> ExecuteSatusRequest(InstructionURI commandURI)
        {
            var status =  new Random(Guid.NewGuid().GetHashCode()).Next(5) switch
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

        public async Task<IResult<InstructionURI>> ExecuteCommand(Command1 command)
        {
            Console.WriteLine($"{typeof(Command1)} handler executed.");
            return IResult<InstructionURI>.Of(command.InstructionURI);
        }
    }
}
