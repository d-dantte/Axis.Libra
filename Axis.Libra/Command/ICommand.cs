namespace Axis.Libra.Command
{
    /// <summary>
    /// A command. 
    /// <para>
    /// Note that all concrete implementations of this interface MUST be decorated with the <see cref="Utils.BindCommandResultAttribute"/> attribute
    /// </para>
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// A unique signature representing this command, typically built by getting a hash of the Command's properties.
        /// </summary>
        string CommandSignature { get; }
    }
}
