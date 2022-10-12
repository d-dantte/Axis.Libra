using Axis.Luna.Extensions;
using HashDepot;
using System;
using System.Text.RegularExpressions;

namespace Axis.Libra.URI
{
    /// <summary>
    /// Represents the uri of an instruction. It takes the general form <c>qry:sample:name:space:here/f0f-a102-ffce558d</c>
    /// </summary>
    public struct InstructionURI
    {
        /// <summary>
        /// <see cref="Regex"/> that describes the pattern for instruction ids
        /// </summary>
        public static readonly Regex InstructionIdPattern = new Regex("^[a-fA-F0-9]{2,4}\\-[a-fA-F0-9]{2,4}\\-[a-fA-F0-9]{4,8}$");

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
        public string Id { get; }


        public InstructionURI(
            Scheme scheme,
            InstructionNamespace @namespace,
            byte[] instructionData)
            :this(
                 scheme,
                 @namespace,
                 XXHash.Hash64(instructionData ?? throw new ArgumentNullException(nameof(instructionData))))
        {
        }

        public InstructionURI(
            Scheme scheme,
            InstructionNamespace @namespace,
            ulong instructionHash)
            :this(scheme, @namespace, instructionHash.ToSignatureHashString())
        {
        }

        public InstructionURI(
            Scheme scheme,
            InstructionNamespace @namespace,
            string instructionId)
        {
            Scheme = scheme;

            Id = instructionId.VerifySignatureHashString();

            Namespace = @namespace.ThrowIfDefault(new ArgumentException($"Invalid {nameof(@namespace)} supplied"));
        }

        public override string ToString()
        {
            return Scheme != null // default(InstructionURI).Scheme is null
                ? $"{Scheme.Value.ToSchemeCode()}:{Namespace}/{Id}"
                : "*";
        }

        public override int GetHashCode() => HashCode.Combine(Scheme, Namespace, Id);

        public override bool Equals(object obj)
        {
            return obj is InstructionURI other
                && other.Scheme == Scheme
                && other.Namespace.Equals(Namespace)
                && other.Id.IsNullOrEqual(Id);
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
                (var @namespace, var idhex) = uriValue[4..]
                    .Split('/')
                    .Map(parts => (parts[0], parts[1]));

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
