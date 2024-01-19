using Axis.Libra.Event;

namespace Axis.Libra.Tests.TestCQRs.Events
{
    public record Event2
    {
        public bool Succeed { get; set; }
    }

    public class Event2Handler_1 : IDomainEventHandler<Event2>
    {
        public async Task Notify(Event2 @event, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(@event));

            Console.WriteLine($"{this.GetType()} notified");
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Cancellation requested");
                return;
            }
            else if (@event.Succeed)
                return;

            else
            {
                await Task.Delay(1, cancellationToken);
                throw new InvalidOperationException("Some exception");
            }

        }
    }

    public class Event2Handler_2 : IDomainEventHandler<Event2>
    {
        public async Task Notify(Event2 @event, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(@event));

            await Task.Delay(10, cancellationToken);
            Console.WriteLine($"{this.GetType()} notified");
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Cancellation requested");
                return;
            }
            else if (@event.Succeed)
                return;

            else
            {
                throw new InvalidOperationException("Some exception");
            }

        }
    }
}
