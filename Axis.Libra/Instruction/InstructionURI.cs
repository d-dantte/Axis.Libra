using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Instruction
{
    /// <summary>
    /// Represents the uri of an instruction. It takes the general form <c>qry:some.name.space/f0fa102ffce5</c>
    /// </summary>
    public readonly struct InstructionURI:
        IDefaultValueProvider<InstructionURI>
    {

        /// <summary>
        /// The scheme for the uri. This value is <c>null</c> for the default <see cref="InstructionURI"/>
        /// </summary>
        public Scheme? Scheme { get; }

        /// <summary>
        /// The namespace for the uri
        /// </summary>
        public InstructionNamespace Namespace { get; }

        /// <summary>
        /// The instruction ID for the uri
        /// </summary>
        public InstructionHash Hash { get; }

        public bool IsDefault =>
            Scheme is null
            && Namespace.IsDefault
            && Hash.IsDefault;

        public static InstructionURI Default => default;

        public InstructionURI(
            Scheme scheme,
            InstructionNamespace @namespace,
            InstructionHash instructionHash)
        {
            Scheme = scheme;

            Hash = instructionHash;

            Namespace = @namespace.ThrowIfDefault(_ => new ArgumentException(
                $"Invalid {nameof(@namespace)}: null"));
        }

        public override string ToString()
        {
            return Scheme is not null
                ? $"{Scheme.Value.ToSchemeCode()}::{Namespace}#{Hash}"
                : "*";
        }

        public override int GetHashCode() => HashCode.Combine(Scheme, Namespace, Hash);

        public override bool Equals(object? obj)
        {
            return obj is InstructionURI other
                && other.Scheme == Scheme
                && other.Namespace.Equals(Namespace)
                && other.Hash.Equals(Hash);
        }

        public static InstructionURI Parse(string uriValue)
        {
            if (string.IsNullOrWhiteSpace(uriValue))
                throw new ArgumentException($"Invalid {nameof(uriValue)}");

            else if (uriValue.Equals("*"))
                return default;

            else
            {
                var scheme = uriValue[..3].ToScheme();
                (var @namespace, var idhex) = uriValue[5..]
                    .Split('#')
                    .ApplyTo(parts => (parts[0], parts[1]));

                return new InstructionURI(scheme, @namespace, idhex);
            }
        }

        public static bool TryParse(string uriValue, out InstructionURI uri)
        {
            try
            {
                uri = Parse(uriValue);
                return true;
            }
            catch
            {
                uri = default;
                return false;
            }
        }


        public static bool operator ==(InstructionURI first, InstructionURI second) => first.Equals(second);

        public static bool operator !=(InstructionURI first, InstructionURI second) => !(first == second);

        public static implicit operator InstructionURI(string uriValue) => Parse(uriValue);
    }
}
