using Axis.Libra.Command;
using Axis.Luna.Operation;
using System;

namespace Axis.Libra.Tests.SampleAssembly.Commands
{
    public class MiscCommand1: AbstractCommand
    {
        public string Stuff { get; set; }

        public override string Namespace => this.GetType().FullName;
    }

    public class MiscCommand2: AbstractCommand
    {
        public string Bleh { get; set; }

        public override string Namespace => this.GetType().FullName;
    }

    public class MiscCommandsHandler :
        ICommandHandler<MiscCommand1>,
        ICommandHandler<MiscCommand2>
    {
        public Operation<string> ExecuteCommand(MiscCommand1 command) => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // do some work...

            return command.CommandURI;
        });

        public Operation<string> ExecuteCommand(MiscCommand2 command) => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // do some work...

            return command.CommandURI;
        });
    }

}
