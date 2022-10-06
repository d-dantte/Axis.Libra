using Axis.Libra.Command;
using Axis.Luna.Operation;
using System;

namespace Axis.Libra.Tests.SampleAssembly.Commands
{
    public class Command1//: AbstractCommand
    {
    }

    //public class Command1Handler : ICommandHandler<Command1>
    //{
    //    public Operation<string> ExecuteCommand(Command1 command) => Operation.Try(() =>
    //    {
    //        if (command == null)
    //            throw new ArgumentNullException(nameof(command));

    //        // do some work...

    //        return command.CommandURI;
    //    });
    //}
}
