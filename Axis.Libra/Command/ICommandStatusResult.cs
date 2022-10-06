using Axis.Libra.Query;
using Axis.Libra.URI;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Represens the outcome of querying the result of a dispatched command.
    /// <para>
    /// All commands should have an implementation of this, registered with the <see cref="QueryRegistrar"/> using <c>IQueryHandler&lt;<see cref="CommandStatusQuery"/>, {CustomCommandStatusResult}&gt;</c>.
    /// The status of the commands can then be queried via the registered handler.
    /// </para>
    /// </summary>
    public interface ICommandStatusResult: IQueryResult
    {
        /// <summary>
        /// The status of the target command.
        /// </summary>
        ICommandStatus Status { get; }

        /// <summary>
        /// The URI of the target command whose status is being sought.
        /// </summary>
        InstructionURI CommandURI { get; }
    }
}
