﻿using Axis.Luna.FInvoke;
using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Immutable;
using Axis.Luna.Extensions;
using HashDepot;

namespace Axis.Libra.Utils
{
    public static class PropertySerializer
    {
        private static readonly ConcurrentDictionary<Type, ImmutableArray<(string name, Func<object, object[], object> func)>> CachedPropertyAccessors = new();


        /// <summary>
        /// Serializes the properties of the given instance, then creates a hash of the byte stream
        /// </summary>
        public static ulong HashProperties(
            object instructionInstance)
            => PropertySerializer
                .Serialize(instructionInstance)
                .ApplyTo(bytes => XXHash.Hash64(bytes));

        /// <summary>
        /// Returns a byte array, typically a concatenation of all the public properties and the class name of the current instance of the query.
        /// Each value is converted to a byte array, and concatenated together.
        /// <para>
        /// The serialization algorithm is as follows:
        /// <list type="number">
        ///     <item>Get all the public instance properties of the object.</item>
        ///     <item>Order the properties in ascending order of the property name (note that names are case sensitive).</item>
        ///     <item>
        ///     For each property, convert the property name to unicode bytes and concatenate with the result of 
        ///     converting the property value to bytes according to their type:
        ///     <list type="number">
        ///         <item>null: ignore</item>
        ///         <item>string: convert to Unicode bytes</item>
        ///         <item>primitives: convert to string, then convert to Unicode bytes</item>
        ///         <item>IEnumerable: recursively call <c>Serialize</c>for each item in the sequence</item>
        ///         <item>all other values: recursively call <c>Serialize</c></item>
        ///     </list>
        ///     </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>a byte array containing the concatenation of converting available property names and valeus to bytes</returns>
        public static byte[] Serialize(object obj, params string[] excludedProperties)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (obj is Type type)
                return type
                    .AssemblyQualifiedName
                    .ApplyTo(aqn => Encoding.Unicode.GetBytes(aqn ?? ""));

            var excludedNames = new HashSet<string>(excludedProperties ?? Array.Empty<string>());

            return CachedPropertyAccessors
                .GetOrAdd(
                    obj.GetType(),
                    type =>
                    {
                        var properties = type.GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance);

                        if (properties.Length == 0)
                        {
                            if (type.IsValueType)
                                return ImmutableArray.Create<(string name, Func<object, object[], object> func)>((
                                    name: "TypeText",
                                    func: (instance, args) => instance.ToString()!));

                            else //type.IsClass
                                return ImmutableArray.Create<(string name, Func<object, object[], object> func)>((
                                    name: "AQN",
                                    func: (instance, args) => type.AssemblyQualifiedName!));
                        }
                        // else
                        return properties
                            .Where(prop => prop.CanRead)
                            .Select(prop => (name: prop.Name, func: InstanceInvoker.InvokerFor(prop.GetGetMethod()).Func))
                            .ToImmutableArray();
                    })
                .Where(prop => !excludedNames.Contains(prop.name))
                .Select(prop => (prop.name, value: prop.func.Invoke(obj, Array.Empty<object>())))
                .OrderBy(prop => prop.name)
                .SelectMany(prop => Convert(prop.name).Concat(Convert(prop.value)))
                .ToArray();
        }

        private static byte[] Convert(object value)
        {
            return value switch
            {
                null => Array.Empty<byte>(),

                #region All values here not fall back to the '_' case.
                // note that unicode bytes are prefered to obviate the issue of little/big endian architecture machines
                // producing different values when converting to bytes

                byte b => Encoding.Unicode.GetBytes(b.ToString()),
                sbyte sb => Encoding.Unicode.GetBytes(sb.ToString()),
                bool b => Encoding.Unicode.GetBytes(b.ToString()),

                char c => Encoding.Unicode.GetBytes(c.ToString()),

                short s => Encoding.Unicode.GetBytes(s.ToString()),
                ushort us => Encoding.Unicode.GetBytes(us.ToString()),

                int i => Encoding.Unicode.GetBytes(i.ToString()),
                uint ui => Encoding.Unicode.GetBytes(ui.ToString()),

                long l => Encoding.Unicode.GetBytes(l.ToString()),
                ulong ul => Encoding.Unicode.GetBytes(ul.ToString()),

                float f => Encoding.Unicode.GetBytes(f.ToString()),
                double d => Encoding.Unicode.GetBytes(d.ToString()),

                decimal d => Encoding.Unicode.GetBytes(d.ToString()),

                string s => Encoding.Unicode.GetBytes(s),
                #endregion

                IEnumerable enm => enm
                    .Cast<object>()
                    .Select(v => Serialize(v))
                    .SelectMany()
                    .ToArray(),

                _ => Serialize(value)
            };
        }
    }
}
