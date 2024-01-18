using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using HashDepot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Libra
{
    internal static class Extensions
    {

        public static string ToSchemeCode(this Scheme scheme)
        {
            return scheme switch
            {
                Scheme.Command => "cmd",
                Scheme.Request => "req",
                Scheme.Query => "qry",
                Scheme.Event => "evt",
                _ => throw new ArgumentException($"Invalid scheme: {scheme}")
            };
        }

        public static Scheme ToScheme(this string scheme)
        {
            return scheme.ToLower() switch
            {
                "cmd" => Scheme.Command,
                "qry" => Scheme.Query,
                "req" => Scheme.Request,
                "evt" => Scheme.Event,
                _ => throw new ArgumentException($"Invalid scheme: {scheme}")
            };
        }

        internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this
            IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs,
            bool ignoreDuplicates)
            where TKey: notnull
        {
            ArgumentNullException.ThrowIfNull(keyValuePairs);

            var result = new Dictionary<TKey, TValue>();
            foreach(var pair in keyValuePairs)
            {
                if (!result.TryAdd(pair.Key, pair.Value)
                    && !ignoreDuplicates)
                    throw new ArgumentException(
                        $"Invalid {nameof(keyValuePairs)}: contains duplicate key '{pair.Key}'");
            }

            return result;
        }



        /// <summary>
        /// Creates a namespace out of the fully qualified name of the instruction type.
        /// </summary>
        internal static InstructionNamespace DefaultInstructionNamespace(this
            Type instructionType,
            string instructionKind)
        {
            var typeNamespace = instructionType.Namespace ?? $"User.{instructionKind}";
            var assemblyQualifiedNameHash = instructionType
                .AssemblyQualifiedName!
                .ThrowIfNull(() => new InvalidOperationException($"Invalid {instructionKind} type {instructionType}"))
                .ApplyTo(Encoding.Unicode.GetBytes)
                .ApplyTo(bytes => XXHash.Hash64(bytes));

            return $"{typeNamespace}@{assemblyQualifiedNameHash:x}";
        }

    }
}
