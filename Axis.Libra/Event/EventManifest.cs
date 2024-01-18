using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axis.Libra.Event
{
    /// <summary>
    /// 
    /// </summary>
    public class EventManifestBuilder
    {
        private readonly Dictionary<Type, HashSet<Type>> _eventHandlerMap = new();
        private Options _options;

        public EventManifestBuilder()
        { }

        public static EventManifestBuilder NewBuilder() => new();

        public EventManifestBuilder WithOptions(Options options)
        {
            _options = options.ThrowIfDefault(_ => new ArgumentException(
                $"Invalid {nameof(options)}: default"));
            return this;
        }

        public EventManifestBuilder AddEventHandler<TEvent, TEventHandler>()
            where TEvent: IDomainEvent
            where TEventHandler: IDomainEventHandler<TEvent>
        {
            _eventHandlerMap
                .GetOrAdd(typeof(TEvent), _ => new HashSet<Type>())
                .Add(typeof(TEventHandler))
                .ThrowIf(false, _ => new ArgumentException(
                    $"Invalid handler type: duplicate value '{typeof(TEventHandler)}'"));
            return this;
        }

        public EventManifest BuildManifest()
        {
            return new EventManifest(
                _options,
                _eventHandlerMap.Select(kvp => new EventInfo(
                    kvp.Key,
                    kvp.Value)));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class EventManifest: IDisposable
    {
        private readonly Dictionary<Type, EventInfo> _eventMap = new();
        private bool _isDisposed = false;

        /// <summary>
        /// The task factory that creates tasks used in the notification broadcast process
        /// </summary>
        internal TaskFactory TaskFactory { get; }

        /// <summary>
        /// Notification broadcast options
        /// </summary>
        public Options EventNotificationOptions { get; }

        internal EventManifest(
            Options options,
            IEnumerable<EventInfo> infoList)
        {
            EventNotificationOptions = options.ThrowIfDefault(
                _ => new ArgumentException($"Invalid {options}: default"));

            TaskFactory = new TaskFactory(
                options.CancellationTokenSource.Token,
                options.TaskCreationOptions,
                TaskContinuationOptions.None,
                options.TaskScheduler);

            _eventMap = infoList
                .ThrowIfNull(() => new ArgumentNullException(nameof(infoList)))
                .ThrowIfAny(
                    _info => _info.IsDefault,
                    _ => new ArgumentException($"Invalid {infoList}: contains default"))
                .ToDictionary(
                    _info => _info.EventType,
                    _info => _info);
        }

        /// <summary>
        /// Gets all the event types registered
        /// </summary>
        public ImmutableArray<Type> EventTypes()
        {
            if (_isDisposed)
                throw new InvalidOperationException($"Invalid state: disposed");

            return _eventMap.Keys.ToImmutableArray();
        }

        /// <summary>
        /// Gets the event info for the given event
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        internal EventInfo? GetEventInfo<TEvent>() where TEvent : IDomainEvent
        {
            if (_isDisposed)
                throw new InvalidOperationException($"Invalid state: disposed");

            return _eventMap.TryGetValue(typeof(TEvent), out var eventInfo)
                ? eventInfo
                : null;
        }

        /// <summary>
        /// Uses the parent <see cref="CancellationTokenSource"/> to abort ALL broadcast sessions.
        /// </summary>
        public void Dispose()
        {
            try
            {
                EventNotificationOptions.CancellationTokenSource.Cancel();
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }
}
