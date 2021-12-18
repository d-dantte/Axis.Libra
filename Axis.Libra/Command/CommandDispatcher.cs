using Axis.Libra.Exceptions;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using System;

namespace Axis.Libra.Command
{
    public class CommandDispatcher
    {
        private readonly IServiceResolver _serviceResolver;

        public CommandDispatcher(IServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        /// <summary>
        /// Dispatches the command to be handled immediately by the registered handler
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public Operation<string> Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return this
                .HandlerFor<TCommand>()
                ?.ExecuteCommand(command)
                ?? throw new RegistrationNotFoundException(typeof(ICommandHandler<TCommand>));
        });

        private ICommandHandler<TCommand> HandlerFor<TCommand>()
        where TCommand : ICommand => _serviceResolver.Resolve<ICommandHandler<TCommand>>();
    }
}
