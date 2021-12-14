using Axis.Luna.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Command
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<TCommand>
    where TCommand: ICommand
    {
        Operation<CommandResult> ExecuteCommand(TCommand command);
    }
}
