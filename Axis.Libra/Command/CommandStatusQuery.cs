using Axis.Libra.Query;
using Axis.Libra.URI;

namespace Axis.Libra.Command
{
    /// <summary>
    /// All commands will be queried using an instance of this type, by supplying the command URI in its constructor, and dispatching it via the <see cref="QueryDispatcher"/>.
    /// <para>
    /// Note that since Handler registrations have to be unique, it is recommmended that to differentiate different command status handlers, implementers should implement the
    /// <see cref="ICommandStatusResult"/> for each command, and register it's handler using <c>IQueryHandler&lt;<see cref="CommandStatusQuery"/>, CustomCommandStatusResult&gt;</c>
    /// </para>
    /// </summary>
    //[InstructionNamespace(nameof(Axis) + "." + nameof(Libra) + "." + nameof(Command) + "." + nameof(CommandStatusQuery))]
    [InstructionNamespace("Axis.Libra.Command.CommandStatusQuery")]
    public sealed class CommandStatusQuery : AbstractQuery
    {
        /// <summary>
        /// The URI of the command whose result is being queried
        /// </summary>
        public InstructionURI CommandURI { get; }

        public CommandStatusQuery(InstructionURI commandURI)
        {
            CommandURI = commandURI;
        }
    }
}
