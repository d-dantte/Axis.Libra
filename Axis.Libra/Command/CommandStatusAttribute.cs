using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Attribute that decorates commands. Used to specify the Runtime type of the commands status result instance.
    /// <para>All concrete commands MUST be decorated with this attribute. It is validated during the registration phase</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class CommandStatusAttribute: Attribute
    {
        /// <summary>
        /// The command status runtime type
        /// </summary>
        public Type CommandStatusType { get; }

        public CommandStatusAttribute(Type commandStatusType)
        {
            CommandStatusType = commandStatusType.ThrowIf(
                IsNotCommandStatusResultType,
                new ArgumentException($"The supplied type is not a proper implementation of {typeof(ICommandStatusResult)}"));
        }

        private static bool IsNotCommandStatusResultType(Type type)
        {
            return type.IsAbstract
                || !(type.IsValueType || type.IsClass)
                || !type.Implements(typeof(ICommandStatusResult));
        }
    }
}
