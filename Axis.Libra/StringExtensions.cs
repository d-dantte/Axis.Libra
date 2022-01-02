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
                throw new ArgumentException(nameof(child));

            if (string.IsNullOrWhiteSpace(parent))
                throw new ArgumentException(nameof(parent));

            return parent.Equals(child, StringComparison.InvariantCulture)
                || child.StartsWith($"{parent}.");
        }
    }
}
