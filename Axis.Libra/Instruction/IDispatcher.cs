using Axis.Libra.Command;
using Axis.Libra.Event;
using Axis.Libra.Exceptions;
using Axis.Libra.Query;
using Axis.Libra.Request;
using Axis.Luna.Extensions;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Axis.Libra.Instruction
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDispatcher
    {
        #region Command
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        InstructionURI DispatchCommand<TCommand>(TCommand command);

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

        #region Event
        /// <summary>
        /// Broadcasts the event to all the registered listeners.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="event">The event instance</param>
        /// <returns>A cancellation source instance that can be used to abort the event broadcast process</returns>
        public CancellationTokenSource BroadcastEvent<TEvent>(
            TEvent @event)
            where TEvent : IDomainEvent;
        #endregion
    }

    internal class DefaultDispatcher: IDispatcher
    {
        private readonly IResolverContract _resolver;
        private readonly CommandManifest _commandManifest;
        private readonly QueryManifest _queryManifest;
        private readonly RequestManifest _requestManifest;
        private readonly EventManifest _eventManifest;

        internal DefaultDispatcher(
            IResolverContract resolver,
            CommandManifest commandManifest,
            QueryManifest queryManifest,
            RequestManifest requestManifest,
            EventManifest eventManifest)
        {
            ArgumentNullException.ThrowIfNull(resolver);
            ArgumentNullException.ThrowIfNull(commandManifest);
            ArgumentNullException.ThrowIfNull(queryManifest);
            ArgumentNullException.ThrowIfNull(requestManifest);
            ArgumentNullException.ThrowIfNull(eventManifest);

            _resolver = resolver;
            _commandManifest = commandManifest;
            _queryManifest = queryManifest;
            _requestManifest = requestManifest;
            _eventManifest = eventManifest;
        }

        #region Command
        public InstructionURI DispatchCommand<TCommand>(TCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            var info = _commandManifest
                .GetCommandInfo<TCommand>()
                ?? throw new UnregisteredInstructionException(command);

            var handler = _resolver
                .Resolve(info.HandlerType)
                .ThrowIfNull(() => new UnresolvedHandlerException(info.HandlerType))!;

            var uri = new InstructionURI(
                Scheme.Command,
                info.Namespace,
                info.Hasher.Invoke(command));

            // fire and forget
            _ = info.Handler.Invoke(command, uri, handler);

            return uri;
        }

        public async Task<ICommandStatus> DispatchCommandStatusRequest(InstructionURI instructionURI)
        {
            if (instructionURI.IsDefault)
                throw new ArgumentException(
                    $"Invalid {nameof(instructionURI)}: default");

            var info = _commandManifest
                .GetCommandInfo(instructionURI.Namespace)
                ?? throw new UnregisteredNamespaceException(instructionURI.Namespace);

            var handler = _resolver
                .Resolve(info.StatusRequestHandlerType)
                .ThrowIfNull(() => new UnresolvedHandlerException(info.StatusRequestHandlerType))!;

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
                ?? throw new UnregisteredInstructionException(query);

            var handler = _resolver
                .Resolve(info.HandlerType)
                .ThrowIfNull(() => new UnresolvedHandlerException(info.HandlerType))!;

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
        public TResult DispatchRequest<TRequest, TResult>(TRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var info = _requestManifest
                .GetRequestInfo<TRequest>()
                ?? throw new UnregisteredInstructionException(request);

            var handler = _resolver
                .Resolve(info.HandlerType)
                .ThrowIfNull(() => new UnresolvedHandlerException(info.HandlerType))!;

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

        #region Event
        public CancellationTokenSource BroadcastEvent<TEvent>(
            TEvent @event)
            where TEvent: IDomainEvent
        {
            ArgumentNullException.ThrowIfNull(@event);

            var info = _eventManifest
                .GetEventInfo<TEvent>()
                ?? throw new UnregisteredInstructionException(@event);

            // Resolve handlers
            var unresolvedHandlerErrors = new List<UnresolvedHandlerException>();
            var resolvedHandlers = new List<IDomainEventHandler<TEvent>>();
            for (int indx = 0; indx < info.HandlerTypes.Length; indx++)
            {
                var handlerType = info.HandlerTypes[indx];
                var handler = _resolver
                    .Resolve(handlerType)
                    .As<IDomainEventHandler<TEvent>>();

                if (handler is null)
                    unresolvedHandlerErrors.Add(new UnresolvedHandlerException(handlerType));

                else resolvedHandlers.Add(handler);
            }

            // Abort if there are some unresolved handlers
            if (unresolvedHandlerErrors.Count > 0)
                throw new AggregateException(unresolvedHandlerErrors);

            // Create a child token source used to abort THIS broadcast sessions.
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _eventManifest.EventNotificationOptions.CancellationTokenSource.Token);

            // Broadcast
            _eventManifest.TaskFactory.StartNew(
                cancellationToken: tokenSource.Token,
                action: () =>
                {
                    for (int cnt = 0; cnt < resolvedHandlers.Count; cnt++)
                    {
                        try
                        {
                            // Fire and forget
                            _ = resolvedHandlers[cnt].Notify(@event, tokenSource.Token);
                        }
                        catch { }
                    }
                });

            return tokenSource;
        }
        #endregion
    }
}
