using System.Threading;
using System.Threading.Tasks;

namespace Axis.Libra.Event
{
    /// <summary>
    /// Represents a handler for the event type <typeparamref name="TEvent"/>
    /// </summary>
    /// <typeparam name="TEvent">The event type this handler is designated to handle</typeparam>
    public interface IDomainEventHandler<TEvent>
    {
        /// <summary>
        /// Respond to the given event.
        /// <para/>
        /// NOTE: This method is executed in a "fire-and-forget" fashion, as such:
        /// <list type="number">
        /// <item>
        ///     Exceptions should not be thrown before the <see cref="Task"/> is returned, as it will be ignored.
        /// </item>
        /// <item>
        ///     The returned task is also ignored, so failures within the task must also be handled by user code.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="event">The event to respond to</param>
        /// <param name="cancellationToken">Cancellation token for the event notification process</param>
        /// <returns></returns>
        Task Notify(TEvent @event, CancellationToken cancellationToken);
    }
}
