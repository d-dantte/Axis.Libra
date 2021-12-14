using Axis.Luna.Extensions;
using System;
using System.Linq;

namespace Axis.Libra.Utils
{
    public static class Extensions
    {
        public static string ToSignatureString(this long value)
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
                });
        }

        public static string ToSignatureString(this ulong value)
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
                });
        }
    }
}
