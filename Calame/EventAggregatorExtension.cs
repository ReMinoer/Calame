using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
            eventAggregator.Subscribe(subscriber, async task =>
            {
                // If already on UI thread, just await
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    await task();
                    return;
                }

                try
                {
                    await task();
                }
                catch (OperationCanceledException)
                {
                    // Rethrow cancellation to publisher
                    throw;
                }
                catch (Exception ex)
                {
                    // Throw on UI thread if messaging throw
                    Execute.BeginOnUIThread(() => ExceptionDispatchInfo.Capture(ex).Throw());
                }
            });
        }
        
        static public void SubscribeOnUI(this IEventAggregator eventAggregator, object subscriber)
        {
            eventAggregator.Subscribe(subscriber, task =>
            {
                // If already on UI thread, just return the task
                if (Application.Current.Dispatcher.CheckAccess())
                    return task();

                var taskCompletionSource = new TaskCompletionSource<bool>();

                Execute.BeginOnUIThread(async () =>
                {
                    // Let messaging throw on UI thread (just keep cancellation)
                    try
                    {
                        await task();
                        taskCompletionSource.SetResult(true);
                    }
                    catch (OperationCanceledException)
                    {
                        taskCompletionSource.SetCanceled();
                    }
                });

                return taskCompletionSource.Task;
            });
        }
    }
}