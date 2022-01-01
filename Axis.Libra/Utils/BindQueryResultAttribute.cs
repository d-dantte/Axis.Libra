using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Utils
{
    /// <summary>
    /// Every <see cref="Query.IQuery"/> instance must be decorated with an instance of this attribute, effectively letting the system know what result type will be returned.
    /// The result type must be an instance of <see cref="Query.IQueryResult"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class BindQueryResultAttribute: Attribute
    {
        /// <summary>
        /// The <see cref="Type"/> of the query's result.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        /// Creates a new instance of this attribute, given a type that implements <see cref="Query.IQueryResult"/>
        /// </summary>
        public BindQueryResultAttribute(Type resultType)
        {
            ResultType = resultType
                .ThrowIfNull(new ArgumentNullException(nameof(resultType)))
                .ThrowIf(t => !t.Implements(typeof(Query.IQueryResult)), new ArgumentException($"Result type must implement {nameof(Query.IQueryResult)}"))
                .ThrowIf(t => t.IsAbstract, new ArgumentException($"Result type must not be abstract"));
        }
    }
}
