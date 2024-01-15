using Axis.Libra.Instruction;
using Axis.Luna.Common.Results;
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
        public Task<IResult<InstructionURI>> Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand
        {
            ArgumentNullException.ThrowIfNull(nameof(command));

            return _manifest
                .HandlerFor<TCommand>()
                ?.ExecuteCommand(command)
                ?? throw new InvalidOperationException(
                    $"Invalid {nameof(command)}: No handler found for '{typeof(TCommand)}'");
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
                throw new ArgumentException($"Invalid {nameof(commandUri)}: default");

            if (commandUri.Scheme != Scheme.Command)
                throw new ArgumentException($"Invalid command uri scheme: {commandUri.Scheme}");

            return _manifest
                .StatusHandlerFor(commandUri.Namespace)
                ?.ExecuteSatusRequest(commandUri)
                ?? throw new InvalidOperationException(
                    $"Invalid Could not find a status handler for the namespace: {commandUri.Namespace}");
        }
    }
}
