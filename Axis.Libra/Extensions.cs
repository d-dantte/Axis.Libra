using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Axis.Libra
{
    internal static class Extensions
    {
        private static readonly ConcurrentDictionary<Type, InstructionNamespace> NamespaceCache = new();

        public static InstructionNamespace InstructionNamespace(this IInstruction instruction) => instruction.GetType().InstructionNamespace();

        public static InstructionNamespace InstructionNamespace(this Type instructionType)
        {
            ArgumentNullException.ThrowIfNull(instructionType);

            return NamespaceCache.GetOrAdd(instructionType, type =>
            {
                // search for the static "InstructionNamespace" method on the type, then call it.
                var methodInfo = instructionType
                    .GetStaticInstructionNamespaceMethod()
                    .ThrowIfNull(() => new InvalidOperationException(
                        $"Invalid {nameof(instructionType)}: no static {nameof(IInstruction.InstructionNamespace)} method."));

                // using regular reflection call because this is only ever done once, and then cached. The overhead of creating
                // a delegate, using it once, and then discarding it, doesn't seem like a worthwhile endeavor.
                return methodInfo!
                    .Invoke(null, Array.Empty<object>())
                    .As<InstructionNamespace>();
            });
        }

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

        private static MethodInfo? GetStaticInstructionNamespaceMethod(this Type type)
        {
            if (type is null || typeof(object).Equals(type))
                return null;

            var method = type.GetMethod(
                nameof(IInstruction.InstructionNamespace),
                BindingFlags.Public | BindingFlags.Static);

            return method switch
            {
                MethodInfo => method,
                _ => type.BaseType!.GetStaticInstructionNamespaceMethod()
            };
        }
    }
}
