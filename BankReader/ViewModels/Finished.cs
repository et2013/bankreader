using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace BankReader.ViewModels
{
    public class Finished : IResult
    {
        public Finished()
        {
        }

        public Finished(Task backgroundTask)
        {
            BackgroundTask = backgroundTask;
        }

        private Task BackgroundTask { get; }

        public void Execute(CoroutineExecutionContext context)
        {
            // Add a continuation task to the main task to notify Completion of this task
            BackgroundTask.ContinueWith(tsk => MessageBox.Show(tsk.Exception.Flatten().Message), CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(tsk => Completed(this, new ResultCompletionEventArgs()));
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}