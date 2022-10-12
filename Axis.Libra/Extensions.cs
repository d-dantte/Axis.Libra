using Axis.Libra.URI;
using Axis.Luna.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Libra
{
    internal static class Extensions
    {
        private static readonly ConcurrentDictionary<Type, string> NamespaceCache = new ConcurrentDictionary<Type, string>();

        public static string ToSignatureHashString(this ulong value)
        {
            return BitConverter
                .GetBytes(value)
                .Select((_byte, index) => index.ValuePair(Convert.ToString(_byte, 16)))
                .Aggregate("", (@string, next) =>
                {
                    return @string += next.Key switch
                    {
                        2 => $"-{next.Value}",
                        4 => $"-{next.Value}",
                        _ => next.Value
                    };
                })
                .ToUpper();
        }

        public static string VerifySignatureHashString(this string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            else if (!InstructionURI.InstructionIdPattern.IsMatch(id))
                throw new ArgumentException($"Invalid id format: {id}");

            else return id;
        }

        public static TOut Map<TIn, TOut>(this TIn @in, Func<TIn, TOut> mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            else return mapper.Invoke(@in);
        }

        public static InstructionNamespace InstructionNamespace(this IInstruction instruction) => instruction.GetType().InstructionNamespace();

        public static InstructionNamespace InstructionNamespace(this Type instructionType)
        {
            return NamespaceCache.GetOrAdd(instructionType, type =>
            {
                return type.GetAttribute<InstructionNamespaceAttribute>()
                    ?.Namespace
                    ?? throw new ArgumentException($"The supplied instruction is not decorated with {typeof(InstructionNamespaceAttribute)}.");
            });
        }

        /// <summary>
        /// Gets the first attribue of the specified type if it exists, or null
        /// </summary>
        internal static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute => type
            .GetCustomAttributes(inherit)
            .Where(attribute => attribute.GetType().Equals(typeof(TAttribute)))
            .FirstOrDefault()
            .As<TAttribute>();

        public static bool HasInstructionNamespace(this Type type) => type.HasAttribute<InstructionNamespaceAttribute>();

        public static string ToSchemeCode(this Scheme scheme)
        {
            return scheme switch
            {
                Scheme.Command => "cmd",
                Scheme.Request => "req",
                Scheme.Query => "qry",
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
                _ => throw new ArgumentException($"Invalid scheme: {scheme}")
            };
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this
            IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs,
            bool ignoreDuplicates)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach(var pair in keyValuePairs)
            {
                if (!result.TryAdd(pair.Key, pair.Value)
                    && !ignoreDuplicates)
                    throw new ArgumentException($"Supplied {nameof(keyValuePairs)} contains duplicate key: {pair.Key}");
            }

            return result;
        }
    }
}
