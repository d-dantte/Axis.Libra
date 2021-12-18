namespace Axis.Libra.Command
{
    /// <summary>
    /// A command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// A unique signature representing this command, typically built by getting a hash of the Command's properties.
        /// </summary>
        string CommandSignature { get; }
    }
}
