using Axis.Libra.URI;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra
{
    /// <summary>
    /// ATtribute that decorates instructions (Queries/Requests/Commands), and specifies the static namespace that the instruction belongs to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class InstructionNamespaceAttribute: Attribute
    {
        /// <summary>
        /// The instruction namespace
        /// </summary>
        public string Namespace { get; }

        public InstructionNamespaceAttribute(string @namespace)
        {
            Namespace = @namespace.ThrowIf(
                InstructionURI.InstructionNamespacePattern.IsNotMatch,
                new ArgumentException($"Invalid namespace format: {@namespace}"));
        }
    }
}
