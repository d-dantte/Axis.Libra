using Axis.Libra.URI;
using Axis.Libra.Utils;
using HashDepot;
using System;

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
        /// <para>
        /// Note that his method excludes the <see cref="ICommand.CommandURI"/> from the serialization to avoid endless recrussion. This follows for any
        /// properties that depend on the <see cref="ICommand.CommandURI"/> property: override this method and exclude them from the serialization.
        /// </para>
        /// </summary>
        /// <returns>byte array representing the class name and property values serialized</returns>
        protected virtual byte[] Serialize() => PropertySerializer.Serialize(
            this,
            nameof(ICommand.CommandURI));

        // <inheritdoc/>
        byte[] IBinarySerializable.Serialize() => Serialize();

        /// <summary>
        /// The command hash is made from concatenating the hash of the command type's full-name, with the result of hashing the outcome of serializing the instance.
        /// </summary>
        public InstructionURI CommandURI => new InstructionURI(Scheme.Command, this.InstructionNamespace(), Serialize());

        public override int GetHashCode() => HashCode.Combine(CommandURI);
    }
}
