using Axis.Libra.Event;

namespace Axis.Libra.Tests.TestCQRs.Events
{
    public record Event1
    {
        public bool Succeed { get; set; }
    }

    public class Event1Handler_1 : IDomainEventHandler<Event1>
    {
        private readonly Action? callback;

        public Event1Handler_1(Action? callback = null)
        {
            this.callback = callback;
        }

        public async Task Notify(Event1 @event, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(@event));

            callback?.Invoke();
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

    public class Event1Handler_2 : IDomainEventHandler<Event1>
    {
        private readonly Action? callback;

        public Event1Handler_2(Action? callback = null)
        {
            this.callback = callback;
        }

        public async Task Notify(Event1 @event, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(@event));

            callback?.Invoke();
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
