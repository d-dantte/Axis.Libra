using Axis.Libra.Command;
using Axis.Libra.Query;
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
        /// Gets the first attribue if it exists, or null
        /// </summary>
        internal static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute => type.GetAttributes<TAttribute>(inherit).FirstOrDefault();

        /// <summary>
        /// Verify that the supplied commandType is a concrete type - a struct or a class.
        /// If this can be expressed as a type constraint, i'd be glad
        /// </summary>
        /// <param name="commandType"></param>
        public static Type ValidateCommandType(this Type commandType)
        {
            return commandType
                .ThrowIfNull(new ArgumentNullException(nameof(commandType)))
                .ThrowIf(t => t.IsAbstract, new Exception($"{commandType} cannot be abstract"))
                // this may be redundant as only structs and classes can implement interfaces
                .ThrowIf(t => !(t.IsClass || t.IsValueType), new Exception($"{commandType} must be a struct or class"))
                .ThrowIf(t => t.GetAttribute<BindCommandResultAttribute>() == null, new Exception($"{commandType} must be bound to an instance of {typeof(ICommandResult)} using the {typeof(BindCommandResultAttribute)}."));
        }

        /// <summary>
        /// Verify that the supplied queryType is a concrete type - a struct or a class.
        /// If this can be expressed as a type constraint, i'd be glad
        /// </summary>
        /// <param name="queryType"></param>
        public static Type ValidateQueryType(this Type queryType)
        {
            return queryType
                .ThrowIfNull(new ArgumentNullException(nameof(queryType)))
                .ThrowIf(t => t.IsAbstract, new Exception($"{queryType} cannot be abstract"))
                // this may be redundant as only structs and classes can implement interfaces
                .ThrowIf(t => !(t.IsClass || t.IsValueType), new Exception($"{queryType} must be a struct or class"));
        }

        /// <summary>
        /// Checks that the given poco is a proper implementation of the <see cref="IQueryHandler{TQuery, TQueryResult}"/> type.
        /// </summary>
        /// <param name="queryHandlerType">the type to validate</param>
        public static Type ValidateQueryHandlerImplementation(this Type queryHandlerType)
        {
            if (queryHandlerType == null)
                throw new ArgumentNullException(nameof(queryHandlerType));

            if (queryHandlerType.IsStructural())
                throw new ArgumentException($"{queryHandlerType} must not be a struct");

            if (queryHandlerType.IsInterface)
                throw new ArgumentException($"{queryHandlerType} must not be an interface");

            if (queryHandlerType.IsAbstract)
                throw new ArgumentException($"{queryHandlerType} must not be abstract");

            if (queryHandlerType.Extends(typeof(Delegate)))
                throw new ArgumentException($"{queryHandlerType} must not be a delegate");

            queryHandlerType
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType)
                .Where(@interface => @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Any()
                .ThrowIf(false, new ArgumentException($"{nameof(queryHandlerType)} must implement {typeof(IQueryHandler<,>)}"));

            return queryHandlerType;
        }

        public static bool TryValidateQueryHandlerImplementation(this Type queryHandlerType)
        {
            try
            {
                _ = queryHandlerType.ValidateQueryHandlerImplementation();
                return true;
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Checks that the given poco is a valid QueryHandler, and also that it's <c>TQuery</c> arg is a <see cref="CommandResultQuery"/>, and it's <c>TQueryResult</c> arg is a <see cref="ICommandResult"/>.
        /// </summary>
        /// <param name="commandResultQueryHandlerType"></param>
        /// <returns></returns>
        public static Type ValidateCommandResultQueryHandlerImplementation(this Type commandResultQueryHandlerType)
        {
            _ = commandResultQueryHandlerType.ValidateQueryHandlerImplementation();
            commandResultQueryHandlerType
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType)
                .Where(@interface => @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Where(@interface =>
                {
                    var gargs = @interface.GetGenericArguments();
                    return gargs[0] == typeof(CommandResultQuery)
                        && gargs[1].Implements(typeof(ICommandResult));
                })
                .Any()
                .ThrowIf(false, new ArgumentException($"{nameof(commandResultQueryHandlerType)} must implement {typeof(IQueryHandler<,>)}"));

            return commandResultQueryHandlerType;
        }

        public static bool TryValidateCommandResultQueryHandlerImplementation(this Type commandResultQueryHandlerType)
        {
            try
            {
                _ = commandResultQueryHandlerType.ValidateCommandResultQueryHandlerImplementation();
                return true;
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Checks that the given poco is a proper implementation of the <see cref="ICommandHandler{TCommand}"/> type.
        /// </summary>
        /// <param name="commandHandlerType">the type to validate</param>
        public static Type ValidateCommandHandlerImplementation(this Type commandHandlerType)
        {
            if (commandHandlerType == null)
                throw new ArgumentNullException(nameof(commandHandlerType));

            if (commandHandlerType.IsStructural())
                throw new ArgumentException($"{commandHandlerType} must not be a struct");

            if (commandHandlerType.IsInterface)
                throw new ArgumentException($"{commandHandlerType} must not be an interface");

            if (commandHandlerType.IsAbstract)
                throw new ArgumentException($"{commandHandlerType} must not be abstract");

            if (commandHandlerType.Extends(typeof(Delegate)))
                throw new ArgumentException($"{commandHandlerType} must not be a delegate");

            commandHandlerType
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType)
                .Where(@interface => @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Any()
                .ThrowIf(false, new ArgumentException($"{nameof(commandHandlerType)} must implement {typeof(ICommandHandler<>)}"));

            return commandHandlerType;
        }

        public static bool TryValidateCommandHandlerImplementation(this Type commandHandlerType)
        {
            try
            {
                _ = commandHandlerType.ValidateCommandHandlerImplementation();
                return true;
            }
            catch
            { }

            return false;
        }
    }
}
