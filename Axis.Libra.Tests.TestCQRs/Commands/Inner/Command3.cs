using Axis.Libra.Command;
using Axis.Libra.Instruction;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using HashDepot;

namespace Axis.Libra.Tests.TestCQRs.Commands.Inner
{
    public class Command3 : ICommand
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

    public class Command3Handler :
        ICommandHandler<Command3>,
        ICommandHandler<Command2>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IResult<ICommandStatus>> ExecuteSatusRequest(InstructionURI commandURI)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
            return Result.Of(status);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IResult<InstructionURI>> ExecuteCommand(Command2 command)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Console.WriteLine($"{typeof(Command1)} handler executed.");
            return Result.Of(command.InstructionURI);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IResult<InstructionURI>> ExecuteCommand(Command3 command)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Console.WriteLine($"{typeof(Command1)} handler executed.");
            return Result.Of(command.InstructionURI);
        }
    }
}
