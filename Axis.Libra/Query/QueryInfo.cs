using Axis.Libra.Instruction;
using Axis.Luna.Common;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Query
{
    public readonly struct QueryInfo :
        IDefaultValueProvider<QueryInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        public InstructionNamespace Namespace { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type QueryType { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// Function that accepts the query, and the handler, and returns a task.
        /// </summary>
        public Func<object, InstructionURI, object, Task> Handler { get; }

        /// <summary>
        /// 
        /// </summary>
        public Func<object, ulong> Hasher { get; }


        public static QueryInfo Default => default;

        public bool IsDefault =>
            Namespace.IsDefault
            && QueryType is null
            && HandlerType is null
            && Handler is null
            && Hasher is null;

        public QueryInfo(
            InstructionNamespace @namespace,
            Type queryType,
            Type handlerType,
            Func<object, InstructionURI, object, Task> handler,
            Func<object, ulong> instructionHasher)
        {
            ArgumentNullException.ThrowIfNull(queryType);
            ArgumentNullException.ThrowIfNull(handlerType);
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentNullException.ThrowIfNull(instructionHasher);

            Namespace = @namespace;
            QueryType = queryType;
            HandlerType = handlerType;
            Handler = handler;
            Hasher = instructionHasher;
        }
    }
}
