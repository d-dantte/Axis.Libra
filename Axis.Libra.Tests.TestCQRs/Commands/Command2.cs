using Axis.Libra.Command;
using Axis.Libra.Instruction;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using HashDepot;
using System.Text;

namespace Axis.Libra.Tests.TestCQRs.Commands
{
    public class Command2
    {
        public TimeSpan TimeToLive { get; set; }

        public static InstructionNamespace InstructionNamespace() => "axis.libra.test-cqrs.command2";

        public InstructionHash InstructionHash()
        {
            return TimeToLive
                .ToString()
                .ApplyTo(Encoding.Unicode.GetBytes)
                .ApplyTo(bytes => XXHash.Hash64(bytes));
        }
    }

    public class Command2Handler
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
    }
}
