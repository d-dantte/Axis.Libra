using Axis.Luna.Common;
using HashDepot;
using System;

namespace Axis.Libra.Instruction
{
    public readonly struct InstructionHash:
        IEquatable<InstructionHash>,
        IDefaultValueProvider<InstructionHash>
    {
        private readonly ulong _hash;

        public static InstructionHash Default => default;

        public bool IsDefault => _hash == 0;

        #region Construction
        public InstructionHash(ulong hash)
        {
            _hash = hash;
        }

        public InstructionHash(
            string hashHex)
            :this(ulong.Parse(hashHex, System.Globalization.NumberStyles.HexNumber))
        {
        }

        public InstructionHash(
            byte[] instructionData)
            :this(XXHash.Hash64(instructionData
                ?? throw new ArgumentNullException(nameof(instructionData))))
        {
        }

        public static implicit operator InstructionHash(ulong hash) => new(hash);

        public static implicit operator InstructionHash(byte[] instructionData) => new(instructionData);

        public static implicit operator InstructionHash(string hahsHex) => new(hahsHex);
        #endregion

        public bool Equals(
            InstructionHash other)
            => _hash == other._hash;

        public override bool Equals(object? obj)
        {
            return obj is InstructionHash hash
                && Equals(hash);
        }

        public static bool operator ==(
            InstructionHash left,
            InstructionHash right)
            => left.Equals(right);

        public static bool operator !=(
            InstructionHash left,
            InstructionHash right)
            => !left.Equals(right);

        public override string ToString() => _hash.ToString("X");

        public override int GetHashCode() => _hash.GetHashCode();
    }
}
