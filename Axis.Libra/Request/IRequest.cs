using Axis.Libra.URI;

namespace Axis.Libra.Request
{
    /// <summary>
    /// A request. Represents instructions (parameters) that cause a system to perform some SYNCHRONIOUS action that
    /// either returns a status, or a result - essentially, mashing the functionalities of the <see cref="Query.IQuery"/> and that of the <see cref="Command.ICommand"/>.
    /// </summary>
    public interface IRequest : IInstruction
    {
        /// <summary>
        /// A unique signature representing this request, in the following format: <code>req:&lt;namespace&gt;/&lt;property hash&gt;</code>
        /// </summary>
        InstructionURI RequestURI { get; }
    }
}
