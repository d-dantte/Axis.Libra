using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Represents the status of a dispatched command at any given time
    /// </summary>
    public interface ICommandStatus
    {
        public static ICommandStatus OfSuccess() => default(Succeeded);

        public static ICommandStatus OfBusy() => default(Busy);

        public static ICommandStatus OfError(string message = null) => new Error(message);

        public static ICommandStatus OfUnknown() => default(Unknown);

        public static ICommandStatus OfProgress(decimal percentage) => new Progress(percentage);

        public static ICommandStatus Of(string value)
        {
            return value?.ToLower() switch
            {
                "succeeded" => OfSuccess(),

                "busy" => OfBusy(),

                "unknown" => OfUnknown(),

                "error" => OfError(),

                null => throw new ArgumentNullException("Invalid command status: null"),

                _ =>
                    value.StartsWith("Error:") ? OfError(value[6..]) :
                    value.EndsWith('%') ? OfProgress(decimal.Parse(value[..^1])) :
                    throw new ArgumentException($"invalid command status: {value}")
            };
        }

        #region Union Types

        #region Success
        /// <summary>
        /// Command completed and succeeded.
        /// <para>
        /// Note that a <c>record struct</c> would have been ideal to implement this.
        /// </para>
        /// </summary>
        public struct Succeeded: ICommandStatus
        {
            public override string ToString() => "Succeeded";

            #region record struct
            public override int GetHashCode() => 0;

            public override bool Equals(object obj) => obj is Succeeded;

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
        public struct Busy : ICommandStatus
        {
            public override string ToString() => "Busy";

            #region record struct
            public override int GetHashCode() => 0;

            public override bool Equals(object obj) => obj is Busy;

            public static bool operator ==(Busy a, Busy b) => a.Equals(b);

            public static bool operator !=(Busy a, Busy b) => !(a == b);
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
        public struct Error : ICommandStatus
        {
            /// <summary>
            /// A message associated with this error, retrieved from the source.
            /// </summary>
            public string Message { get; }

            public Error(string message) => Message = message;

            public override string ToString() => "Error" + (string.IsNullOrEmpty(Message) ? "" : $":{Message}");

            #region record struct
            public override int GetHashCode() => Message?.GetHashCode() ?? 0;

            public override bool Equals(object obj)
            {
                return obj is Error err
                    && err.Message.IsNullOrEqual(Message, System.StringComparison.InvariantCulture);
            }

            public static bool operator ==(Error a, Error b) => a.Equals(b);

            public static bool operator !=(Error a, Error b) => !(a == b);
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
        public struct Unknown : ICommandStatus
        {
            public override string ToString() => "Unknown";

            #region record struct
            public override int GetHashCode() => 0;

            public override bool Equals(object obj) => obj is Unknown;

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
        public struct Progress : ICommandStatus
        {
            /// <summary>
            /// A positive value, between 0 and 100 (inclusive), indicating the percentage progress of the command.
            /// </summary>
            public decimal Percentage { get; }

            public Progress(decimal percentage)
            {
                Percentage = percentage.ThrowIf(
                    v => v < 0m || v > 100m,
                    new ArgumentException($"{nameof(percentage)} value must be between 0 and 100, inclusive. Value supplied: {percentage}"));
            }

            public override string ToString() => $"{Percentage}%";

            #region record struct
            public override int GetHashCode() => Percentage.GetHashCode();

            public override bool Equals(object obj)
            {
                return obj is Progress progress
                    && progress.Percentage.Equals(Percentage);
            }

            public static bool operator ==(Progress a, Progress b) => a.Equals(b);

            public static bool operator !=(Progress a, Progress b) => !(a == b);
            #endregion
        }
        #endregion

        #endregion
    }
}
