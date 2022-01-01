using Axis.Libra.Query;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// All commands will be queried using an instance of this type, by supplying the command signature in its constructor, and dispatching it via the <see cref="QueryDispatcher"/>.
    /// </summary>
    public sealed class CommandResultQuery: AbstractQuery
    {
        /// <summary>
        /// The signature of the command whose result is being queried
        /// </summary>
        public string CommandSignature { get; }

        public CommandResultQuery(string commandSignature)
        {
            CommandSignature = commandSignature.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(commandSignature)}"));
        }
    }

}
