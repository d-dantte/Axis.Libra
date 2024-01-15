using System;

namespace Axis.Libra
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Tests if the given namespaces are equal, or one is a parent of the other
        /// </summary>
        /// <param name="child">child namespace</param>
        /// <param name="parent">parent namespace</param>
        public static bool IsChildNamespaceOf(this string child, string parent)
        {
            if (string.IsNullOrWhiteSpace(child))
                throw new ArgumentException($"Invalid {nameof(child)}: null/whitespace");

            if (string.IsNullOrWhiteSpace(parent))
                throw new ArgumentException($"Invalid {nameof(parent)}: null/whitespace");

            return parent.Equals(child, StringComparison.InvariantCulture)
                || child.StartsWith($"{parent}.");
        }

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
