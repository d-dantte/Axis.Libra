namespace Axis.Libra.Command
{
    /// <summary>
    /// A command. Represents instructions (parameters) that cause a system to act, and ultimately modify it's internal state ASYNCHRONIOUSLY.
    /// </summary>
    public interface ICommand: IInstruction
    {
    }
}
