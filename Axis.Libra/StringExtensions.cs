using System;

namespace Axis.Libra
{
    internal static class StringExtensions
    {
        public static bool IsNullOrEqual(this
            string? first,
            string? second,
            StringComparison stringComparison = StringComparison.InvariantCulture)
        {
            if (first is null && second is null)
                return true;

            else return first?.Equals(second, stringComparison) ?? false;
        }
    }
}
