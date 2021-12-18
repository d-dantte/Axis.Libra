using Axis.Libra.Query;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Represens the result of querying the result of a dispatched command.
    /// </summary>
    public interface ICommandResult: IQueryResult
    {
        CommandStatus Status { get; }

        string CommandSignature { get; }
    }


    /// <summary>
    /// "Default" implementation of the <see cref="ICommandResult"/>. This implementation returns no real result, besides announcing the <see cref="CommandStatus"/>
    /// of the dispatched command.
    /// </summary>
    public readonly struct CommandStatusResult : ICommandResult
    {
        public string CommandSignature { get; }
        public string QuerySignature { get; }
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
