using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Axis.Libra.Utils
{
    public static class PropertySerializer
    {
        /// <summary>
        /// Returns a byte array, typically a concatenation of all the public properties and the class name of the current instance of the query.
        /// Each value is converted to a byte array, and concatenated together.
        /// </summary>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            return obj
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.CanRead)
                .Select(prop => InstanceInvoker.InvokerFor(prop.GetGetMethod()))
                .Select(invoker => invoker.Func.Invoke(obj, Array.Empty<object>()))
                .Select(Convert)
                .SelectMany()
                .ToArray();
        }

        private static byte[] Convert(object value)
        {
            return value switch
            {
                null => Array.Empty<byte>(),

                short s => BitConverter.GetBytes(s),
                ushort us => BitConverter.GetBytes(us),

                int i => BitConverter.GetBytes(i),
                uint ui => BitConverter.GetBytes(ui),

                long l => BitConverter.GetBytes(l),
                ulong ul => BitConverter.GetBytes(ul),

                float f => BitConverter.GetBytes(f),
                double d => BitConverter.GetBytes(d),

                char c => BitConverter.GetBytes(c),
                bool b => BitConverter.GetBytes(b),

                decimal d => decimal
                    .GetBits(d)
                    .Select(BitConverter.GetBytes)
                    .SelectMany()
                    .ToArray(),

                string s => Encoding.Unicode.GetBytes(s),

                Type t => Encoding.Unicode.GetBytes(t.FullName),

                IBinarySerializable bs => bs.Serialize(),

                _ => Encoding.Unicode.GetBytes(value.ToString())
            };
        }
    }
}
