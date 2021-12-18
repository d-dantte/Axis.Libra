using Axis.Libra.Query;
using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using System;

namespace Axis.Libra.Command
{
    /// <summary>
    /// All commands will be queried using an instance of this type, by supplying the command signature in its constructor, and dispatching it via the <see cref="QueryDispatcher"/>.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public sealed class CommandResultQuery<TCommand>: AbstractQuery
    where TCommand : ICommand
    {
        /// <summary>
        /// The type value of the <see cref="TCommand"/> generic type
        /// </summary>
        public Type CommandType => typeof(TCommand);

        /// <summary>
        /// The signature of the command whose result is being queried
        /// </summary>
        public string CommandSignature { get; }

        public CommandResultQuery(string commandSignature)
        {
            ValidateCommandType();

            CommandSignature = commandSignature.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(commandSignature)}"));
        }

        /// <summary>
        /// Verify that <see cref="TCommand"/> is a concrete type - a struct or a class.
        /// If this can be expressed as a type constraint, i'd be glad
        /// </summary>
        private void ValidateCommandType()
        {
            typeof(TCommand)
                .ThrowIf(t => t.IsAbstract, new Exception($"{typeof(TCommand)} cannot be abstract"))
                // this may be redundant as only structs and classes can implement interfaces
                .ThrowIf(t => !(t.IsClass || t.IsValueType), new Exception($"{typeof(TCommand)} must be a struct or class"))
                .ThrowIf(t => t.GetAttribute<BindResultAttribute>() == null, new Exception($"{typeof(TCommand)} must be bound to an instance of {typeof(ICommandResult)} using the {typeof(BindResultAttribute)}."));
        }
    }

}
