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
                .ThrowIf(t => !(t.IsClass || t.IsValueType), new Exception($"{queryType} must be a struct or class"))
                .ThrowIf(t => 
                    t.GetAttribute<BindQueryResultAttribute>() == null,
                    new Exception($"{queryType} must be bound to an instance of {typeof(IQueryResult)} using the {typeof(BindQueryResultAttribute)}."));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericDefinitionInterface"></param>
        /// <returns></returns>
        public static Type GetGenericInterface(this Type type, Type genericDefinitionInterface)
        {
            genericDefinitionInterface
                .ThrowIf(t => !t.IsGenericTypeDefinition, new ArgumentException("interface is not a generic type definition"))
                .ThrowIf(t => !t.IsInterface, new ArgumentException($"supplied {nameof(genericDefinitionInterface)} type is not an interface"));

            return type
                .GetInterfaces()
                .Where(_i => _i.IsGenericType)
                .Where(_i => _i.GetGenericTypeDefinition() == genericDefinitionInterface)
                .FirstOrDefault();
        }
    }
}
