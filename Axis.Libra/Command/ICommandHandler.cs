using Axis.Luna.Operation;

namespace Axis.Libra.Command
{
    /// <summary>
    /// A command handler encapsualtes logic that processes a specific <see cref="ICommand"/> instance.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command handled by this handler</typeparam>
    public interface ICommandHandler<in TCommand>
    where TCommand: ICommand
    {
        /// <summary>
        /// Executes the command and returns a string representing the command's signature. This signature can be used to create a <see cref="CommandResultQuery{TCommand}"/>
        /// object to query for the commands status/result.
        /// </summary>
        /// <param name="command">The command instance</param>
        /// <returns>The command's signature</returns>
        Operation<string> ExecuteCommand(TCommand command);
    }
}
