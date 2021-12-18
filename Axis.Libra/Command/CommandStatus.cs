namespace Axis.Libra.Command
{
    /// <summary>
    /// Represents the status of a dispatched command at any given time
    /// </summary>
    public enum CommandStatus
    {
        /// <summary>
        /// Command completed and succeeded
        /// </summary>
        Succeeded,

        /// <summary>
        /// Command is not completed
        /// </summary>
        Busy,

        /// <summary>
        /// Command errored
        /// </summary>
        Errored,

        /// <summary>
        /// Command is unkonwn, typically because it has not been dispatched
        /// </summary>
        Unknown
    }
}
