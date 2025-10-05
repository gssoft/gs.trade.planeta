using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ca_Task_Finish_01
{
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var cancellationCompletionSource = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(() => cancellationCompletionSource.TrySetResult(true)))
            {
                if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }
            return await task;
        }
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var cancellationCompletionSource = new TaskCompletionSource<bool>();

            //using (cancellationToken.Register(() => cancellationCompletionSource.TrySetResult(true)))
            //{
                if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            //}
            // return await task;
        }
    }
}
