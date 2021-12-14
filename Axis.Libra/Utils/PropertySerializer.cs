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

                short => BitConverter.GetBytes((short)value),
                ushort => BitConverter.GetBytes((ushort)value),

                int => BitConverter.GetBytes((int)value),
                uint => BitConverter.GetBytes((uint)value),

                long => BitConverter.GetBytes((long)value),
                ulong => BitConverter.GetBytes((ulong)value),

                float => BitConverter.GetBytes((float)value),
                double => BitConverter.GetBytes((double)value),

                char => BitConverter.GetBytes((char)value),
                bool => BitConverter.GetBytes((bool)value),

                decimal => decimal
                    .GetBits((decimal)value)
                    .Select(BitConverter.GetBytes)
                    .SelectMany()
                    .ToArray(),

                string => Encoding.Unicode.GetBytes(value as string),

                IBinarySerializable => value.As<IBinarySerializable>().Serialize(),

                _ => Encoding.Unicode.GetBytes(value.ToString())
            };
        }
    }
}
