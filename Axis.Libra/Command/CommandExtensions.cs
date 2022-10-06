using Axis.Luna.Extensions;
using System;
using System.Collections.Concurrent;

namespace Axis.Libra.Command
{
    public static class CommandExtensions
    {
        private static readonly ConcurrentDictionary<Type, Type> CommandStatusTypeCache = new ConcurrentDictionary<Type, Type>();

        public static Type CommandStatusType(this ICommand command) => command.GetType().CommandStatusType();

        public static Type CommandStatusType(this Type commandType)
        {
            return CommandStatusTypeCache.GetOrAdd(commandType, type =>
            {
                return type
                    .ThrowIf(IsNotCommandType, new ArgumentException($"{commandType} is not an implementatoin of {typeof(ICommand)}"))
                    .GetAttribute<CommandStatusAttribute>()
                    ?.CommandStatusType
                    ?? throw new ArgumentException($"The supplied commandType is not decorated with {typeof(CommandStatusAttribute)}.");
            });
        }

        private static bool IsNotCommandType(Type type)
        {
            return type.IsAbstract
                || !(type.IsValueType || type.IsClass)
                || !type.Implements(typeof(ICommand));
        }
    }
}
