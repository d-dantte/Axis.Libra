using Axis.Luna.Operation;
using System;
using System.Collections.Generic;

namespace Axis.Libra.Command
{
    public class CommandDispatcher
    {
        private readonly Dictionary<Type, object> _handlers = new Dictionary<Type, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool RegisterrHandler<TCommand>(ICommandHandler<TCommand> handler)
        where TCommand : ICommand
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_handlers.ContainsKey(typeof(TCommand)))
                return false;

            _handlers.Add(typeof(TCommand), handler);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int HandlerCount() => _handlers.Count;

        /// <summary>
        /// Returns an enumerable of <c>TCommands</c>
        /// </summary>
        public IEnumerable<Type> RegisteredCommands() => _handlers.Keys;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public Operation<CommandResult> Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var handler = HandlerFor<TCommand>();

            /// At this point, we can call into filter logic that'll support AOP-like cross-cutting features like logging, etc...

            return handler.ExecuteCommand(command);
        });

        private ICommandHandler<TCommand> HandlerFor<TCommand>()
        where TCommand : ICommand => _handlers.ContainsKey(typeof(TCommand))
            ? _handlers[typeof(TCommand)] as ICommandHandler<TCommand>
            : throw new Exceptions.RegistrationNotFoundException(typeof(ICommandHandler<TCommand>));
    }
}
