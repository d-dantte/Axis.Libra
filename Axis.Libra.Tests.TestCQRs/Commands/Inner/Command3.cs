using Axis.Libra.Command;
using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using HashDepot;

namespace Axis.Libra.Tests.TestCQRs.Commands.Inner
{
    public class Command3
    {
        public Guid Id { get; set; }

        public static InstructionNamespace InstructionNamespace() => "axis.libra.test-cqrs.command3";

        public InstructionHash InstructionHash()
        {
            return Id
                .ToByteArray()
                .ApplyTo(bytes => XXHash.Hash64(bytes));
        }
    }

    public class Command3Handler
    {
        public async Task<ICommandStatus> ExecuteSatusRequest(InstructionURI commandURI)
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
            await Task.Delay(1);
            return status;
        }

        public async Task ExecuteCommand(Command2 command)
        {
            Console.WriteLine($"{typeof(Command2)} handler executed.");
            await Task.Delay(1);
        }

        public async Task ExecuteCommand(Command3 command)
        {
            Console.WriteLine($"{typeof(Command3)} handler executed.");
            await Task.Delay(1);
        }
    }
}
