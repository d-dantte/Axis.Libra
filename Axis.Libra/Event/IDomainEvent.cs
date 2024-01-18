using Axis.Libra.Instruction;

namespace Axis.Libra.Event
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static abstract InstructionNamespace EventNamespace();
    }
}
