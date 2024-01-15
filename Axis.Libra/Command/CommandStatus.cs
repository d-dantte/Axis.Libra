using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Represents the status of a dispatched command at any given time
    /// </summary>
    public interface ICommandStatus
    {
        public static ICommandStatus OfSuccess(InstructionURI commandURI) => new Succeeded(commandURI);

        public static ICommandStatus OfBusy(InstructionURI commandURI) => new Busy(commandURI);

        public static ICommandStatus OfError(InstructionURI commandURI, string? message = null) => new Error(commandURI, message);

        public static ICommandStatus OfUnknown(InstructionURI commandURI) => new Unknown(commandURI);

        public static ICommandStatus OfProgress(InstructionURI commandURI, decimal percentage) => new Progress(commandURI, percentage);

        public static ICommandStatus Of(InstructionURI commandURI, string value)
        {
            return value?.ToLower() switch
            {
                "succeeded" => OfSuccess(commandURI),

                "busy" => OfBusy(commandURI),

                "unknown" => OfUnknown(commandURI),

                "error" => OfError(commandURI),

                null => throw new ArgumentNullException(nameof(value)),

                _ =>
                    value.StartsWith("Error:") ? OfError(commandURI, value[6..]) :
                    value.EndsWith('%') ? OfProgress(commandURI, decimal.Parse(value[..^1])) :
                    throw new ArgumentException($"Invalid command status: {value}")
            };
        }

        #region Members
        InstructionURI CommandURI { get; }
        #endregion

        #region Union Types

        #region Success
        /// <summary>
        /// Command completed and succeeded.
        /// <para>
        /// Note that a <c>record struct</c> would have been ideal to implement this.
        /// </para>
        /// </summary>
        public readonly struct Succeeded: ICommandStatus
        {
            public InstructionURI CommandURI { get; }

            internal Succeeded(InstructionURI commandUri)
            {
                CommandURI = commandUri
                    .ThrowIfDefault(
                        _ => new ArgumentException($"Invalid uri: {commandUri}"))
                    .ThrowIf(
                        uri => uri.Scheme != Scheme.Command,
                        _ => new ArgumentException($"uri scheme must be {Scheme.Command}"));
            }

            public override string ToString() => $"{nameof(Succeeded)}[{CommandURI}]";

            #region record struct
            public override int GetHashCode() => HashCode.Combine(CommandURI);

            public override bool Equals(
                object? obj)
                => obj is Succeeded other
                && other.CommandURI.Equals(CommandURI);

            public static bool operator ==(Succeeded a, Succeeded b) => a.Equals(b);

            public static bool operator !=(Succeeded a, Succeeded b) => !(a == b);
            #endregion
        }
        #endregion

        #region Busy
        /// <summary>
        /// Command is not YET completed, and it's progress is unknowable.
        /// <para>
        /// Note that a <c>record struct</c> would have been ideal to implement this.
        /// </para>
        /// </summary>
        public readonly struct Busy : ICommandStatus
        {
            public InstructionURI CommandURI { get; }

            internal Busy(InstructionURI commandUri)
            {
                CommandURI = commandUri
                    .ThrowIfDefault(_ => new ArgumentException($"Invalid uri: {commandUri}"))
                    .ThrowIf(
                        uri => uri.Scheme != Scheme.Command,
                        _ => new ArgumentException($"uri scheme must be {Scheme.Command}"));
            }

            public override string ToString() => $"{nameof(Busy)}[{CommandURI}]";

            #region record struct
            public override int GetHashCode() => HashCode.Combine(CommandURI);

            public override bool Equals(
                object? obj)
                => obj is Busy other
                && other.CommandURI.Equals(CommandURI);

            public static bool operator ==(Busy a, Busy b) => a.Equals(b);

            public static bool operator !=(Busy a, Busy b) => !(a == b);
            #endregion
        }
        #endregion

        #region Unknown
        /// <summary>
        /// Command status is unkonwn, typically because it has not been dispatched
        /// <para>
        /// Note that a <c>record struct</c> would have been ideal to implement this.
        /// </para>
        /// </summary>
        public readonly struct Unknown : ICommandStatus
        {
            public InstructionURI CommandURI { get; }

            internal Unknown(InstructionURI commandUri)
            {
                CommandURI = commandUri
                    .ThrowIfDefault(_ => new ArgumentException($"Invalid uri: {commandUri}"))
                    .ThrowIf(
                        uri => uri.Scheme != Scheme.Command,
                        _ => new ArgumentException($"uri scheme must be {Scheme.Command}"));
            }

            public override string ToString() => $"{nameof(Unknown)}[{CommandURI}]";

            #region record struct
            public override int GetHashCode() => HashCode.Combine(CommandURI);

            public override bool Equals(
                object? obj)
                => obj is Unknown other
                && other.CommandURI.Equals(CommandURI);

            public static bool operator ==(Unknown a, Unknown b) => a.Equals(b);

            public static bool operator !=(Unknown a, Unknown b) => !(a == b);
            #endregion
        }
        #endregion

        #region Progress
        /// <summary>
        /// Command is currently busy, with the exact progress represented in a percentage value.
        /// <para>
        /// Note that a <c>record struct</c> would have been ideal to implement this.
        /// </para>
        /// </summary>
        public readonly struct Progress : ICommandStatus
        {
            /// <summary>
            /// A positive value, between 0 and 100 (inclusive), indicating the percentage progress of the command.
            /// </summary>
            public decimal Percentage { get; }

            public InstructionURI CommandURI { get; }

            internal Progress(InstructionURI commandUri, decimal percentage)
            {
                Percentage = percentage.ThrowIf(
                    v => v < 0m || v > 100m,
                    _ => new ArgumentException(
                        $"Invalid {nameof(percentage)}: value '{percentage}' must be between 0 and 100, inclusive."));

                CommandURI = commandUri
                    .ThrowIfDefault(_ => new ArgumentException($"Invalid uri: {commandUri}"))
                    .ThrowIf(
                        uri => uri.Scheme != Scheme.Command,
                        _ => new ArgumentException($"uri scheme must be {Scheme.Command}"));
            }

            public override string ToString() => $"{Percentage}% [{CommandURI}]";

            #region record struct
            public override int GetHashCode() => HashCode.Combine(CommandURI, Percentage);

            public override bool Equals(object? obj)
            {
                return obj is Progress progress
                    && progress.Percentage.Equals(Percentage)
                    && progress.CommandURI.Equals(CommandURI);
            }

            public static bool operator ==(Progress a, Progress b) => a.Equals(b);

            public static bool operator !=(Progress a, Progress b) => !(a == b);
            #endregion
        }
        #endregion

        #region Error
        /// <summary>
        /// Command errored
        /// <para>
        /// Note that a <c>record struct</c> would have been ideal to implement this.
        /// </para>
        /// </summary>
        public readonly struct Error : ICommandStatus
        {
            /// <summary>
            /// A message associated with this error, retrieved from the source.
            /// </summary>
            public string? Message { get; }

            public InstructionURI CommandURI { get; }

            internal Error(InstructionURI commandUri, string? message)
            {
                Message = message;
                CommandURI = commandUri
                    .ThrowIfDefault(_ => new ArgumentException($"Invalid uri: {commandUri}"))
                    .ThrowIf(
                        uri => uri.Scheme != Scheme.Command,
                        _ => new ArgumentException($"uri scheme must be {Scheme.Command}"));
            }

            public override string ToString()
                => $"{nameof(Error)}[{CommandURI}]"
                + (string.IsNullOrEmpty(Message) ? "" : $": {Message}");

            #region record struct
            public override int GetHashCode() => HashCode.Combine(CommandURI, Message);

            public override bool Equals(object? obj)
            {
                return obj is Error err
                    && err.Message.IsNullOrEqual(Message, StringComparison.InvariantCulture)
                    && err.CommandURI.Equals(CommandURI);
            }

            public static bool operator ==(Error a, Error b) => a.Equals(b);

            public static bool operator !=(Error a, Error b) => !(a == b);
            #endregion
        }
        #endregion

        #endregion
    }
}
