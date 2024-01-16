using Axis.Libra.Instruction;
using Axis.Luna.Common;
using System;

namespace Axis.Libra.Request
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct RequestInfo :
        IDefaultValueProvider<RequestInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        public InstructionNamespace Namespace { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type RequestType { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// Function that accepts the request, and the handler, and returns a task.
        /// </summary>
        public Func<object, InstructionURI, object, object> Handler { get; }

        /// <summary>
        /// 
        /// </summary>
        public Func<object, ulong> Hasher { get; }

        public static RequestInfo Default => default;

        public bool IsDefault =>
            Namespace.IsDefault
            && RequestType is null
            && HandlerType is null
            && Handler is null
            && Hasher is null;

        public RequestInfo(
            InstructionNamespace @namespace,
            Type requestType,
            Type handlerType,
            Func<object, InstructionURI, object, object> handler,
            Func<object, ulong> instructionHasher)
        {
            ArgumentNullException.ThrowIfNull(requestType);
            ArgumentNullException.ThrowIfNull(handlerType);
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentNullException.ThrowIfNull(instructionHasher);

            Namespace = @namespace;
            RequestType = requestType;
            HandlerType = handlerType;
            Handler = handler;
            Hasher = instructionHasher;
        }
    }
}
