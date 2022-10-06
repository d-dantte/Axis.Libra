using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Axis.Libra.Utils
{
    public static class PropertySerializer
    {
        private static readonly ConcurrentDictionary<Type, List<(string, InstanceInvoker)>> CachedPropertyAccessors = new ConcurrentDictionary<Type, List<(string name, InstanceInvoker invoker)>>();

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
        ///         <item>number, char, bool: convert to string, then convert to Unicode bytes</item>
        ///         <item>c# type: get the <c>Type.FullName</c> of the type, and convert to Unicode bytes</item>
        ///         <item>IBinarySerializable: call the serialize method</item>
        ///         <item>IEnumerable: repeat the process for each type, then concatenate the resultant bytes</item>
        ///         <item>_ : for all other types, call "ToString()", and convert to Unicode bytes</item>
        ///     </list>
        ///     </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>a byte array containing the concatenation of converting available property names and valeus to bytes</returns>
        public static byte[] Serialize(object obj, params string[] excludedProperties)
        {
            if (obj is null)
                throw new ArgumentNullException($"Invalid '{nameof(obj)}' argument");

            var excludedNames = new HashSet<string>(excludedProperties ?? Array.Empty<string>());

            return CachedPropertyAccessors
                .GetOrAdd(
                    obj.GetType(),
                    type => type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.CanRead)
                        .Select(prop => (name: prop.Name, invoker: InstanceInvoker.InvokerFor(prop.GetGetMethod())))
                        .ToList())
                .Where(prop => !excludedNames.Contains(prop.Item1))
                .Select(prop => (name: prop.Item1, value: prop.Item2.Func.Invoke(obj, Array.Empty<object>())))
                .OrderBy(prop => prop.name)
                .SelectMany(prop => Convert(prop.name).Concat(Convert(prop.value)))
                .SelectMany()
                .ToArray();
        }

        private static byte[] Convert(object value)
        {
            return value switch
            {
                null => Array.Empty<byte>(),

                #region Can be moved to the '_' case.
                byte b => Encoding.Unicode.GetBytes(b.ToString()),
                sbyte sb => Encoding.Unicode.GetBytes(sb.ToString()),

                char c => Encoding.Unicode.GetBytes(c.ToString()),
                bool b => Encoding.Unicode.GetBytes(b.ToString()),

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

                Type t => Encoding.Unicode.GetBytes(t.FullName),

                IBinarySerializable bs => bs.Serialize(),

                IEnumerable enm => enm
                    .Cast<object>()
                    .Select(Convert)
                    .SelectMany()
                    .ToArray(),

                _ => Encoding.Unicode.GetBytes(value.ToString())
            };
        }
    }
}
