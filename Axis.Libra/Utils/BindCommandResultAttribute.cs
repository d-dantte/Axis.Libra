using Axis.Libra.Command;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Utils
{
    /// <summary>
    /// Every <see cref="Command.ICommand"/> instance must be decorated with an instance of this attribute, effectively letting the system know what result type will be returned when
    /// the decorated command's result is queried. The result type must be an instance of <see cref="ICommandResult"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class BindCommandResultAttribute: Attribute
    {
        /// <summary>
        /// The <see cref="Type"/> of the command's result.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        /// Creates a new instance of this attribute, given a type that implements <see cref="ICommandResult"/>
        /// </summary>
        public BindCommandResultAttribute(Type resultType)
        {
            ResultType = resultType
                .ThrowIfNull(new ArgumentNullException(nameof(resultType)))
                .ThrowIf(t => !t.Implements(typeof(ICommandResult)), new ArgumentException($"Result type must implement {nameof(ICommandResult)}"))
                .ThrowIf(t => t.IsAbstract, new ArgumentException($"Result type must not be abstract"));
        }
    }
}
