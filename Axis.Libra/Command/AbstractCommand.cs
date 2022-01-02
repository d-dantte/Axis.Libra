using Axis.Libra.Utils;
using HashDepot;
using System;
using System.Text;

namespace Axis.Libra.Command
{
    public abstract class AbstractCommand : ICommand, IBinarySerializable
    {

        /// <summary>
        /// Appends the serialized class name to the bytes representing the serialized property values
        /// Implementations of this class can override the default implementation of this method to include or remove properties as seen fit.
        /// <para>
        /// Ultimately, the aim of this method is to generate a unique ID representing this command. The uniqueness of commands depends on use-case.
        /// </para>
        /// </summary>
        /// <returns>byte array representing the class name and property values serialized</returns>
        protected virtual byte[] Serialize() => PropertySerializer.Serialize(this);

        byte[] IBinarySerializable.Serialize() => Serialize();

        /// <summary>
        /// The command hash is made from concatenating the hash of the command type's full-name, with the result of hashing the outcome of serializing the instance.
        /// </summary>
        public string CommandSignature
        {
            get
            {
                var nameHash = XXHash.Hash64(Encoding.Unicode.GetBytes(this.GetType().FullName));
                var typeHash = XXHash.Hash64(Serialize());

                return $"{nameHash.ToSignatureString()}/{typeHash.ToSignatureString()}";
            }
        }

        public override int GetHashCode() => HashCode.Combine(CommandSignature);
    }
}
