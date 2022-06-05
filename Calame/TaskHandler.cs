using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Action = System.Action;

namespace Calame
{
    static public class TaskHandler
    {
        static public async Task Handle(Func<Task> task)
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
        }

        static public Task HandleOnUIThread(Func<Task> task)
        {
            // If already on UI thread, just return the task
            if (Application.Current.Dispatcher.CheckAccess())
                return task();

            var taskCompletionSource = new TaskCompletionSource<bool>();

            Execute.BeginOnUIThread(async () =>
            {
                // Let messaging throw on UI thread (just keep the cancellation for publisher)
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
        }
    }
}