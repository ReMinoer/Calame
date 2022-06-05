using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Calame
{
    static public class EventAggregatorExtension
    {
        static public Task PublishAsync(this IEventAggregator eventAggregator, object message) => eventAggregator.PublishAsync(message, CancellationToken.None);
        static public Task PublishAsync(this IEventAggregator eventAggregator, object message, CancellationToken cancellationToken)
        {
            return eventAggregator.PublishOnCurrentThreadAsync(message, cancellationToken);
        }

        static public void Subscribe(this IEventAggregator eventAggregator, object subscriber)
        {
            eventAggregator.Subscribe(subscriber, TaskHandler.Handle);
        }
        
        static public void SubscribeOnUI(this IEventAggregator eventAggregator, object subscriber)
        {
            eventAggregator.Subscribe(subscriber, TaskHandler.HandleOnUIThread);
        }
    }
}