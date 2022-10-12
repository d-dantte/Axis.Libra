using Axis.Libra.Exceptions;
using Axis.Libra.URI;
using Axis.Luna.Common;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Command
{
    public class CommandDispatcher
    {
        private readonly CommandManifest _manifest;

        public CommandDispatcher(CommandManifest manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        /// <summary>
        /// Dispatches the command to be handled immediately by the registered handler.
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns>An <see cref="Operation{TResult}"/> encapsulating the command signature used to query for it's results</returns>
        public Task<IResult<InstructionURI>> Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return _manifest
                .HandlerFor<TCommand>()
                ?.ExecuteCommand(command)
                ?? throw new InvalidOperationException($"could not find a handler for the command of type: {typeof(TCommand)}");
        }

        /// <summary>
        /// Dispatches a request for the status of the command identified by the given <paramref name="commandUri"/>.
        /// </summary>
        /// <param name="commandUri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Task<IResult<ICommandStatus>> DispatchStatusRequest(InstructionURI commandUri)
        {
            if (commandUri == default)
                throw new ArgumentException($"Invalid {nameof(commandUri)}: {commandUri}");

            if (commandUri.Scheme != Scheme.Command)
                throw new ArgumentException($"Command URI must have {Scheme.Command} scheme");

            return _manifest
                .StatusHandlerFor(commandUri.Namespace)
                ?.ExecuteSatusRequest(commandUri)
                ?? throw new InvalidOperationException($"Could not find a status handler for the namespace: {commandUri.Namespace}");
        }
    }
}
