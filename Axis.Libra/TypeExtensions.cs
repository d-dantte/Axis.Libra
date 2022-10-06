using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Libra.Request;
using Axis.Libra.Utils;
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
        /// Gets the first attribue of the specified type if it exists, or null
        /// </summary>
        internal static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute => type.GetAttributes<TAttribute>(inherit).FirstOrDefault();

        internal static bool IsDecoratedWith<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute => type.GetAttribute<TAttribute>(inherit) != null;

        internal static bool IsDecoratedWith(this Type type, bool inherit, params Type[] attributeTypes)
            => type.GetCustomAttributes(inherit).ContainsAll(attributeTypes);

        /// <summary>
        /// Verify that the supplied type is a concrete type - a struct or a class, and that it implements
        /// either <see cref="ICommand"/>, <see cref="IQuery"/>, or <see cref="IRequest"/>.
        /// If this can be expressed as a type constraint, i'd be glad
        /// </summary>
        /// <param name="instructionType"></param>
        public static Type ValidateInstructionType(this Type instructionType)
        {
            return instructionType
                .ThrowIfNull(new ArgumentNullException(nameof(instructionType)))
                .ThrowIf(
                    t => t.IsAbstract,
                    new ArgumentException($"{instructionType} cannot be abstract"))
                .ThrowIf(
                    t => !(t.IsClass || t.IsValueType),
                    new ArgumentException($"{instructionType} must be a struct or class"))
                .ThrowIf(
                    t => !t.IsDecoratedWith<InstructionNamespaceAttribute>(),
                    new ArgumentException($"{instructionType} must be decorated with {typeof(InstructionNamespaceAttribute)}."))
                .ThrowIf(
                    t => !t.ImplementsAny(
                        typeof(ICommand),
                        typeof(IQuery),
                        typeof(IRequest)),
                    new Exception($"{instructionType} must implement either {nameof(ICommand)}, {nameof(IQuery)}, or {nameof(IRequest)}"));
        }

        public static Type ValidateCommandStatusInstruction(this Type commandType)
        {
            return commandType
                .ThrowIf(
                    t => !t.IsDecoratedWith<CommandStatusAttribute>(),
                    new ArgumentException($"{commandType} must be decorated with {typeof(CommandStatusAttribute)}"));
        }

        /// <summary>
        /// Checks that the given poco is a proper implementation of the base type: ICommandHandler<>, IQueryHandler<,>, IRequestHandler<>.
        /// </summary>
        /// <param name="handlerType">the type to validate</param>
        private static Type ValidateHandlerImplementation(this Type handlerType, Type handlerGenericTypeDef)
        {
            if (handlerType == null)
                throw new ArgumentNullException(nameof(handlerType));

            if (handlerType.IsValueType)
                throw new ArgumentException($"{handlerType} must not be a struct");

            if (handlerType.IsInterface)
                throw new ArgumentException($"{handlerType} must not be an interface");

            if (handlerType.IsAbstract)
                throw new ArgumentException($"{handlerType} must not be abstract");

            if (handlerType.Extends(typeof(Delegate)))
                throw new ArgumentException($"{handlerType} must not be a delegate");

            if (!handlerType.ImplementsGenericInterface(handlerGenericTypeDef))
                throw new ArgumentException($"{nameof(handlerType)} must implement {handlerGenericTypeDef}");

            return handlerType;
        }

        /// <summary>
        /// Checks that the given poco is a proper implementation of the <see cref="IQueryHandler{TQuery, TQueryResult}"/> type.
        /// </summary>
        /// <param name="queryHandlerType">the type to validate</param>
        public static Type ValidateQueryHandlerImplementation(this Type queryHandlerType)
            => queryHandlerType.ValidateHandlerImplementation(typeof(IQueryHandler<,>));

        /// <summary>
        /// Checks that the given poco is a proper implementation of the <see cref="IRequestHandler{TRequest}"/> type.
        /// </summary>
        /// <param name="requestHandlerType">the type to validate</param>
        public static Type ValidateRequestHandlerImplementation(this Type requestHandlerType)
            => requestHandlerType.ValidateHandlerImplementation(typeof(IRequestHandler<>));

        /// <summary>
        /// Checks that the given poco is a proper implementation of the <see cref="ICommandHandler{TCommand}"/> type.
        /// </summary>
        /// <param name="commandHandlerType">the type to validate</param>
        public static Type ValidateCommandHandlerImplementation(this Type commandHandlerType)
            => commandHandlerType.ValidateHandlerImplementation(typeof(ICommandHandler<>));

        /// <summary>
        /// Verifies that the given type implements any of the supplied interfaces.
        /// </summary>
        /// <param name="type">The type to test against. Can be a class or struct, or interface, etc.</param>
        /// <param name="firstInterface">The first interface to check for implementation</param>
        /// <param name="otherInterfaces">Multiple interfaces to check for implementation</param>
        internal static bool ImplementsAny(this Type type, Type firstInterface, params Type[] otherInterfaces)
        {
            var interfaces = new HashSet<Type>(type.GetInterfaces());
            return firstInterface
                .Concat(otherInterfaces)
                .Distinct()
                .Where(@interface => @interface.IsInterface)
                .Any(interfaces.Contains);
        }
    }
}
