using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.Utils;
using Axis.Luna.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.SampleAssembly.Commands
{

    public class Command2: AbstractCommand
    {
        public string UserId { get; set; }

        public override string Namespace => typeof(Command2).FullName;
    }

    public class Command2Handler : ICommandHandler<Command2>
    {
        public Operation<string> ExecuteCommand(Command2 command) => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // do some work...

            return command.CommandURI;
        });
    }
}
