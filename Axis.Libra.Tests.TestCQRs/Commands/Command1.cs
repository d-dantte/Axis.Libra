using Axis.Libra.Command;
using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using HashDepot;
using System.Text;

namespace Axis.Libra.Tests.TestCQRs.Commands
{
    public class Command1
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public static InstructionNamespace InstructionNamespace() => "axis.libra.test-crs.command1";

        public InstructionHash InstructionHash()
        {
            return $"{Name}:{Description}"
                .ApplyTo(Encoding.Unicode.GetBytes)
                .ApplyTo(bytes => XXHash.Hash64(bytes));
        }
    }

    public class Command1Handler
    {
        private Action? callback;

        public Command1Handler(Action? callback = null)
        {
            this.callback = callback;
        }

        public async Task<ICommandStatus> ExecuteSatusRequest(InstructionURI commandURI)
        {
            callback?.Invoke();
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
            await Task.Delay(1);
            return status;
        }

        public async Task ExecuteCommand(Command1 command)
        {
            callback?.Invoke();
            Console.WriteLine($"{typeof(Command1)} handler executed.");
            await Task.Delay(1);
        }
    }
}
