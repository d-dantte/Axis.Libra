using Axis.Luna.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Axis.Libra.Instruction
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct InstructionNamespace
    {
        /// <summary>
        /// <see cref="Regex"/> that describes the pattern for instruction namespaces
        /// </summary>
        public static readonly Regex NamespacePattern = new("^[a-zA-Z_][\\w-]*(\\.[\\w-]+)*$");

        /// <summary>
        /// The 'unique' name for this namespace
        /// </summary>
        public string Name { get; }

        public InstructionNamespace(string @namespace)
        {
            Name = NamespacePattern.IsMatch(@namespace)
                ? @namespace
                : throw new ArgumentException($"Invalid namespace: {@namespace}. Namespace must match the pattern: /{NamespacePattern}/");
        }

        public override int GetHashCode() => HashCode.Combine(Name);

        public override bool Equals(object? obj)
        {
            return obj is InstructionNamespace other
                && other.Name.NullOrEquals(Name);
        }

        public override string ToString() => Name;

        public static bool operator ==(InstructionNamespace first, InstructionNamespace second) => first.Equals(second);
        public static bool operator !=(InstructionNamespace first, InstructionNamespace second) => !first.Equals(second);

        public static implicit operator InstructionNamespace(string value) => new(value);

        public static implicit operator string(InstructionNamespace value) => value.Name;
    }
}
