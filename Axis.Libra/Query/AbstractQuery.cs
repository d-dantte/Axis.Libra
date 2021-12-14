using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using System.Linq;
using System.Text;

namespace Axis.Libra.Query
{
    public abstract class AbstractQuery : IQuery, IBinarySerializable
    {

        /// <summary>
        /// Appends the serialized class name to the bytes representing the serialized property values
        /// </summary>
        /// <returns>byte array representing the class name and property values serialized</returns>
        protected virtual byte[] Serialize() => Encoding.Unicode
            .GetBytes(this.GetType().FullName)
            .Concat(PropertySerializer.Serialize(this))
            .SelectMany()
            .ToArray();

        byte[] IBinarySerializable.Serialize() => Serialize();

        public string QuerySignature() => HashDepot.XXHash.Hash64(Serialize()).ToSignatureString();
    }
}
