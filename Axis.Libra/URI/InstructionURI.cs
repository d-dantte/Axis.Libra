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
        /// <see cref="Regex"/> that describes the pattern for instruction namespaces
        /// </summary>
        public static readonly Regex InstructionNamespacePattern = new Regex("^[a-zA-Z_\\-][\\w-]*(:[a-zA-Z_\\-][\\w-]*)*$");

        /// <summary>
        /// <see cref="Regex"/> that describes the pattern for instruction ids
        /// </summary>
        public static readonly Regex InstructionIdPattern = new Regex("^[a-fA-F0-9]{2,4}\\-[a-fA-F0-9]{2,4}\\-[a-fA-F0-9]{4,8}$");

        /// <summary>
        /// The scheme for the uri
        /// </summary>
        public string Scheme { get; }

        /// <summary>
        /// The namespace for the uri
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// The instruction ID for the uri
        /// </summary>
        public string Id { get; }


        public InstructionURI(
            Scheme scheme,
            string @namespace,
            byte[] instructionData)
            :this(scheme, @namespace, XXHash.Hash64(instructionData))
        {
        }

        public InstructionURI(
            Scheme scheme,
            string @namespace,
            ulong instructionHash)
            :this(scheme, @namespace, instructionHash.ToSignatureHashString())
        {
        }

        public InstructionURI(Scheme scheme, string @namespace, string instructionId)
        {
            Scheme = scheme switch
            {
                URI.Scheme.Command => "cmd",
                URI.Scheme.Request => "req",
                URI.Scheme.Query => "qry",
                _ => throw new ArgumentException($"Invalid scheme: {scheme}")
            };

            Id = instructionId.VerifySignatureHashString();

            Namespace = InstructionNamespacePattern.IsMatch(@namespace)
                ? @namespace
                : throw new ArgumentException($"Invalid namespace: {@namespace}. Namespace must match the pattern: /{InstructionNamespacePattern}/");
        }

        public override string ToString()
        {
            return Scheme != null
                ? $"{Scheme}:{Namespace}/{Id}"
                : "*";
        }

        public override int GetHashCode() => HashCode.Combine(Scheme, Namespace, Id);

        public override bool Equals(object obj)
        {
            return obj is InstructionURI other
                && other.Scheme.IsNullOrEqual(Scheme)
                && other.Namespace.IsNullOrEqual(Namespace)
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
