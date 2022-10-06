using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.Utils;
using Axis.Luna.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.SampleAssembly.Commands.Soft
{
    /// <summary>
    /// Person data representing command2result. This can be modeled this way, or with the actual data (Name, DOB, Bleh) encapsulated
    /// in another struct/class referenced from the actual CommandResult - i.e using the <see cref="CommandDataResult{Data}"/>
    /// </summary>
    public class PersonData: ICommandResult
    {
        #region ICommandResult
        public ICommandStatus Status { get; set; }

        public string CommandURI { get; set; }

        public string QueryURI { get; set; }
        #endregion
    }

    public class Command4: AbstractCommand
    {
        public string UserId { get; set; }

        public override string Namespace => this.GetType().FullName;
    }

    public class Command4Handler : ICommandHandler<Command4>
    {
        public Operation<string> ExecuteCommand(Command4 command) => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // do some work...

            return command.CommandURI;
        });
    }


    #region Result query
    public class Command4ResultQueryHandler : IQueryHandler<CommandStatusQuery<Command4>, PersonData>
    {
        public Operation<PersonData> ExecuteQuery(CommandStatusQuery<Command4> query) => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new PersonData 
            { 
                CommandURI = query.CommandURI,
                QueryURI = query.QueryURI,
                Status = ICommandStatus.OfSuccess()
            };
        });
    }

    #endregion
}
