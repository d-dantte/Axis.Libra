using Axis.Libra.URI;
using Axis.Luna.Common;
using System.Threading.Tasks;

namespace Axis.Libra.Command
{
    public interface ICommandStatusHandler
    {
        /// <summary>
        /// Handle the status-request for the given command uri
        /// </summary>
        /// <param name="commandURI">The URI of the command whose status is being sought</param>
        /// <returns>The status of the command</returns>
        Task<IResult<ICommandStatus>> ExecuteSatusRequest(InstructionURI commandURI);
    }

    /// <summary>
    /// A command handler encapsualtes logic that processes a specific <see cref="ICommand"/> instance.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command handled by this handler</typeparam>
    public interface ICommandHandler<in TCommand>: ICommandStatusHandler
    where TCommand: ICommand
    {
        /// <summary>
        /// Executes the command and returns a string representing the command's signature. This signature can be used to create a <see cref="CommandResultQuery{TCommand}"/>
        /// object to query for the commands status/result.
        /// </summary>
        /// <param name="command">The command instance</param>
        /// <returns>The command's signature</returns>
        Task<IResult<InstructionURI>> ExecuteCommand(TCommand command);
    }
}
