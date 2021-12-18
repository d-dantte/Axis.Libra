using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Libra
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets all attributes of the given type that exist on the supplied type.
        /// </summary>
        internal static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Type type, bool inherit = false)
        where TAttribute: Attribute
        {
            return type
                .ThrowIfNull(new ArgumentNullException(nameof(type)))
                .GetCustomAttributes(typeof(TAttribute), inherit)
                .Cast<TAttribute>();
        }

        /// <summary>
        /// Gets the first attribue if it exists, or null
        /// </summary>
        internal static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute => type.GetAttributes<TAttribute>(inherit).FirstOrDefault();
    }
}
