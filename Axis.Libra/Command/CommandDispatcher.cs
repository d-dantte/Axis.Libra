using Axis.Libra.Exceptions;
using Axis.Libra.URI;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using System;

namespace Axis.Libra.Command
{
    public class CommandDispatcher
    {
        private readonly IResolverContract _serviceResolver;

        public CommandDispatcher(IResolverContract serviceResolver)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        /// <summary>
        /// Dispatches the command to be handled immediately by the registered handler
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns>An <see cref="Operation{TResult}"/> encapsulating the command signature used to query for it's results</returns>
        public IOperation<InstructionURI> Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand => Operation.Try(() =>
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return this
                .HandlerFor<TCommand>()
                ?.ExecuteCommand(command)
                ?? throw new UnknownResolverException(typeof(ICommandHandler<TCommand>));
        });

        private ICommandHandler<TCommand> HandlerFor<TCommand>()
        where TCommand : ICommand => _serviceResolver.Resolve<ICommandHandler<TCommand>>();
    }
}
