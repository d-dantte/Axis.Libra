using Axis.Libra.URI;
using Axis.Libra.Utils;
using HashDepot;
using System;

namespace Axis.Libra.Request
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractRequest: IRequest, IBinarySerializable
    {
        /// <summary>
        /// Appends the serialized class name to the bytes representing the serialized public property values.
        /// Implementations of this class can override the default implementation of this method to include or remove properties as seen fit.
        /// <para>
        /// Ultimately, the aim of this method is to generate a unique ID representing this request. The uniqueness of requests depends on use-case.
        /// </para>
        /// <para>
        /// Note that his method excludes the <see cref="IRequest.RequestURI"/> from the serialization to avoid endless recrussion.
        /// Implementations of this class having properties that depend on the <see cref="IRequest.RequestURI"/> property are advised to override this method and exclude them from the serialization.
        /// </para>
        /// </summary>
        /// <returns>byte array representing the class name and property values serialized</returns>
        protected virtual byte[] Serialize() => PropertySerializer.Serialize(
            this,
            nameof(IRequest.RequestURI));

        // <inheritdoc />
        byte[] IBinarySerializable.Serialize() => Serialize();

        /// <summary>
        /// The request hash is made from concatenating the hash of the command type's full-name, with the result of hashing the outcome of serializing the instance.
        /// </summary>
        public InstructionURI RequestURI => new InstructionURI(Scheme.Request, this.InstructionNamespace(), Serialize());

        public override int GetHashCode() => HashCode.Combine(RequestURI);
    }
}
