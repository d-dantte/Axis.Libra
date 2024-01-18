using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Axis.Libra.Event
{
    /// <summary>
    /// Encapsulates all information necessary to identify an event and execute it's handlers
    /// </summary>
    internal readonly struct EventInfo:
        IDefaultValueProvider<EventInfo>
    {
        /// <summary>
        /// The event type
        /// </summary>
        public Type EventType { get; }

        /// <summary>
        /// The event handler type
        /// </summary>
        public ImmutableArray<Type> HandlerTypes { get; }

        public bool IsDefault
            => EventType is null
            && default(ImmutableArray<Type>).Equals(HandlerTypes);

        public static EventInfo Default => default;

        public EventInfo(
            Type eventType,
            IEnumerable<Type> handlerTypes)
        {
            ArgumentNullException.ThrowIfNull(eventType);
            ArgumentNullException.ThrowIfNull(handlerTypes);

            EventType = eventType;
            HandlerTypes = handlerTypes
                .ThrowIfAny(
                    t => t is null,
                    _ => new ArgumentException(
                        $"Invalid {nameof(handlerTypes)}: contains null"))
                .ToImmutableArray();
        }
    }
}
