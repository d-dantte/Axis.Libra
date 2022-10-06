using Axis.Libra.URI;
using Axis.Luna.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace Axis.Libra
{
    public static class Extensions
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

        public static bool IsNotMatch(this Regex regex, string @string) => !regex.IsMatch(@string);

        public static string InstructionNamespace(this IInstruction instruction) => instruction.GetType().InstructionNamespace();

        public static string InstructionNamespace(this Type instructionType)
        {
            return NamespaceCache.GetOrAdd(instructionType, type =>
            {
                return type.GetAttribute<InstructionNamespaceAttribute>()
                    ?.Namespace
                    ?? throw new ArgumentException($"The supplied instruction is not decorated with {typeof(InstructionNamespaceAttribute)}.");
            });
        }

        public static T ValueAtOrDefault<T>(this T[] array, int index, out bool found)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if(index < 0 || index >= array.Length)
            {
                found = false;
                return default;
            }

            //else
            found = true;
            return array[index];
        }
    }
}
