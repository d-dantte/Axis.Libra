using Axis.Libra.URI;

namespace Axis.Libra.Command
{
    /// <summary>
    /// A command. Represents instructions (parameters) that cause a system to act, and ultimately modify it's internal state ASYNCHRONIOUSLY.
    /// <para>
    /// Note that all concrete implementations of this interface MUST be decorated with the <see cref="CommandStatusAttribute"/> attribute
    /// </para>
    /// </summary>
    public interface ICommand: IInstruction
    {
        /// <summary>
        /// A unique signature representing this command, in the following format: <code>cmd:&lt;namespace&gt;/&lt;property hash&gt;</code>
        /// </summary>
        InstructionURI CommandURI { get; }
    }
}
