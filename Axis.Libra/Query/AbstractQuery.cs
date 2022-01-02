using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using HashDepot;
using System;
using System.Linq;
using System.Text;

namespace Axis.Libra.Query
{
    public abstract class AbstractQuery : IQuery, IBinarySerializable
    {

        /// <summary>
        /// Appends the serialized class name to the bytes representing the serialized property values.
        /// Implementations of this class can override the default implementation of this method to include or remove properties as seen fit.
        /// <para>
        /// Ultimately, the aim of this method is to generate a unique ID representing this query. The uniqueness of queries depends on use-case.
        /// </para>
        /// </summary>
        /// <returns>byte array representing the class name and property values serialized</returns>
        protected virtual byte[] Serialize() => Encoding.Unicode
            .GetBytes(this.GetType().FullName)
            .Concat(PropertySerializer.Serialize(this))
            .SelectMany()
            .ToArray();

        byte[] IBinarySerializable.Serialize() => Serialize();


        /// <summary>
        /// The query hash is made from concatenating the hash of the command type's full-name, with the result of hashing the outcome of serializing the instance.
        /// </summary>
        public string QuerySignature
        {
            get
            {
                var nameHash = XXHash.Hash64(Encoding.Unicode.GetBytes(this.GetType().FullName));
                var typeHash = XXHash.Hash64(Serialize());

                return $"{nameHash.ToSignatureString()}/{typeHash.ToSignatureString()}";
            }
        }

        public override int GetHashCode() => HashCode.Combine(QuerySignature);
    }
}
