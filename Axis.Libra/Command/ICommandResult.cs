using Axis.Libra.Query;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Represens the outcome of querying the result of a dispatched command.
    /// </summary>
    public interface ICommandResult: IQueryResult
    {
        CommandStatus Status { get; }

        string CommandSignature { get; }
    }


    /// <summary>
    /// "Default" implementation of the <see cref="ICommandResult"/>. This implementation returns no real result, besides announcing the <see cref="CommandStatus"/>
    /// of the dispatched command.
    /// <para>
    /// For commands that do not have any any data result, instances of this struct can be used to report on the status of the command.
    /// </para>
    /// </summary>
    public readonly struct CommandStatusResult : ICommandResult
    {
        /// <summary>
        /// The signature of the command who's result is being queried
        /// </summary>
        public string CommandSignature { get; }

        /// <summary>
        /// The signature of the <see cref="CommandResultQuery"/> instance that was used to initiate the query.
        /// </summary>
        public string QuerySignature { get; }

        /// <summary>
        /// The status of the command
        /// </summary>
        public CommandStatus Status { get; }

        public CommandStatusResult(CommandStatus status, string commandSignature, string querySignature)
        {
            Status = status;

            CommandSignature = commandSignature.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(commandSignature)}"));

            QuerySignature = querySignature.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(querySignature)}"));
        }

        public override int GetHashCode() => HashCode.Combine(Status, CommandSignature, QuerySignature);

        public override bool Equals(object obj)
        {
            return obj is CommandStatusResult other
                && other.Status == Status
                && other.CommandSignature.NullOrEquals(CommandSignature)
                && other.QuerySignature.NullOrEquals(QuerySignature);
        }

        public static bool operator ==(CommandStatusResult first, CommandStatusResult second) => first.Equals(second);

        public static bool operator !=(CommandStatusResult first, CommandStatusResult second) => !(first == second);
    }
}
