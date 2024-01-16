using Axis.Libra.Instruction;
using Axis.Luna.Common;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Command
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct CommandInfo :
        IDefaultValueProvider<CommandInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        public InstructionNamespace Namespace { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type CommandType { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// Function that accepts the command, and the handler, and returns a task.
        /// </summary>
        public Func<object, InstructionURI, object, Task> Handler { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type StatusRequestHandlerType { get; }

        /// <summary>
        /// Function that accepts the command uri, and the handler, and returns a status task.
        /// </summary>
        public Func<InstructionURI, object, Task<ICommandStatus>> StatusRequestHandler { get; }

        /// <summary>
        /// 
        /// </summary>
        public Func<object, ulong> Hasher { get; }

        public static CommandInfo Default => default;

        public bool IsDefault =>
            Namespace.IsDefault
            && CommandType is null
            && HandlerType is null
            && Handler is null
            && StatusRequestHandlerType is null
            && StatusRequestHandler is null
            && Hasher is null;

        public CommandInfo(
            InstructionNamespace @namespace,
            Type commandType,
            Type handlerType,
            Type statusRequestHandlerType,
            Func<object, InstructionURI, object, Task> handler,
            Func<InstructionURI, object, Task<ICommandStatus>> statusRequestHandler,
            Func<object, ulong> instructionHasher)
        {
            ArgumentNullException.ThrowIfNull(commandType);
            ArgumentNullException.ThrowIfNull(handlerType);
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentNullException.ThrowIfNull(statusRequestHandlerType);
            ArgumentNullException.ThrowIfNull(statusRequestHandler);
            ArgumentNullException.ThrowIfNull(instructionHasher);

            Namespace = @namespace;
            CommandType = commandType;
            HandlerType = handlerType;
            Handler = handler;
            StatusRequestHandler = statusRequestHandler;
            StatusRequestHandlerType = statusRequestHandlerType;
            Hasher = instructionHasher;
        }
    }
}
