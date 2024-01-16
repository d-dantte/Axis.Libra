using Axis.Libra.Command;
using Axis.Libra.Exceptions;
using Axis.Libra.Query;
using Axis.Libra.Request;
using Axis.Luna.Extensions;
using Axis.Proteus.IoC;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Instruction
{
    public interface IDispatcher
    {
        #region Command
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<InstructionURI> DispatchCommand<TCommand>(TCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instructionURI"></param>
        /// <returns></returns>
        Task<ICommandStatus> DispatchCommandStatusRequest(InstructionURI instructionURI);
        #endregion

        #region Query
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResult> DispatchQuery<TQuery, TResult>(TQuery query);
        #endregion

        #region Request
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        TResult DispatchRequest<TRequest, TResult>(TRequest request);
        #endregion
    }

    internal class DefaultDispatcher: IDispatcher
    {
        private IResolverContract _resolver;
        private readonly CommandManifest _commandManifest;
        private readonly QueryManifest _queryManifest;
        private readonly RequestManifest _requestManifest;

        internal DefaultDispatcher(
            IResolverContract resolver,
            CommandManifest commandManifest,
            QueryManifest queryManifest,
            RequestManifest requestManifest)
        {
            ArgumentNullException.ThrowIfNull(resolver);
            ArgumentNullException.ThrowIfNull(commandManifest);
            ArgumentNullException.ThrowIfNull(queryManifest);
            ArgumentNullException.ThrowIfNull(requestManifest);

            _resolver = resolver;
            _commandManifest = commandManifest;
            _queryManifest = queryManifest;
            _requestManifest = requestManifest;
        }


        #region Command
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<InstructionURI> DispatchCommand<TCommand>(TCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            var info = _commandManifest
                .GetCommandInfo<TCommand>()
                ?? throw new InvalidOperationException(
                    $"Invalid {nameof(command)}: no registration found for the given command type '{command?.GetType()}'");

            var handler = _resolver
                .Resolve(info.HandlerType)
                .ThrowIfNull(() => new UnregisteredInstructionException(command))!;

            var uri = new InstructionURI(
                Scheme.Command,
                info.Namespace,
                info.Hasher.Invoke(command));

            await info.Handler.Invoke(command, uri, handler);

            return uri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instructionURI"></param>
        /// <returns></returns>
        public async Task<ICommandStatus> DispatchCommandStatusRequest(InstructionURI instructionURI)
        {
            if (instructionURI.IsDefault)
                throw new ArgumentException(
                    $"Invalid {nameof(instructionURI)}: default");

            var info = _commandManifest
                .GetCommandInfo(instructionURI.Namespace)
                ?? throw new InvalidOperationException(
                    $"Invalid instruction namespace: no registration found for the given namespace '{instructionURI.Namespace}'");

            var handler = _resolver
                .Resolve(info.StatusRequestHandlerType)
                .ThrowIfNull(() => new UnregisteredNamespaceException(instructionURI.Namespace))!;

            return await info.StatusRequestHandler.Invoke(instructionURI, handler);
        }
        #endregion

        #region Query
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TResult> DispatchQuery<TQuery, TResult>(TQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var info = _queryManifest
                .GetQueryInfo<TQuery>()
                ?? throw new InvalidOperationException(
                    $"Invalid {nameof(query)}: no registration found for the given query type '{query?.GetType()}'");

            var handler = _resolver
                .Resolve(info.HandlerType)
                .ThrowIfNull(() => new UnregisteredInstructionException(query))!;

            var uri = new InstructionURI(
                Scheme.Query,
                info.Namespace,
                info.Hasher.Invoke(query));

            var result = await info.Handler
                .Invoke(query, uri, handler)
                .As<Task<TResult>>();

            return result;
        }
        #endregion

        #region Request
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public TResult DispatchRequest<TRequest, TResult>(TRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var info = _requestManifest
                .GetRequestInfo<TRequest>()
                ?? throw new InvalidOperationException(
                    $"Invalid {nameof(request)}: no registration found for the given request type '{request?.GetType()}'");

            var handler = _resolver
                .Resolve(info.HandlerType)
                .ThrowIfNull(() => new UnregisteredInstructionException(request))!;

            var uri = new InstructionURI(
                Scheme.Query,
                info.Namespace,
                info.Hasher.Invoke(request));

            var result = info.Handler
                .Invoke(request, uri, handler)
                .As<TResult>();

            return result;
        }
        #endregion
    }
}
