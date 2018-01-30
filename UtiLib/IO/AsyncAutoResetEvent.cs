using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace UtiLib.IO
{
    public class AsyncAutoResetEvent
    {
        private static readonly Task s_completed = Task.FromResult(true);
        private readonly ConcurrentQueue<TaskCompletionSource<bool>> m_waits = new ConcurrentQueue<TaskCompletionSource<bool>>();
        private int m_signaled;

        public AsyncAutoResetEvent(bool isSignaled = false)
        {
            m_signaled = isSignaled ? 1 : 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wait">Time in milliseconds</param>
        /// <returns></returns>
        public Task WaitAsync(int? wait = null)
        {
            if (Interlocked.Exchange(ref m_signaled, 0) == 1)
            {
                return s_completed;
            }
            else
            {
                var tcs = new TaskCompletionSource<bool>();
                if (wait != null && wait > 0)
                {
                    Task.Delay((int)wait).ContinueWith(_ => tcs.TrySetResult(false));
                }
                m_waits.Enqueue(tcs);
                return tcs.Task;
            }
        }

        public bool TestExchange()
        {
            if (Interlocked.Exchange(ref m_signaled, 0) == 1)
            {
                return true;
            }
            return false;
        }

        public void Set()
        {
            if (m_waits.TryDequeue(out var toRelease))
            {
                if (!toRelease.TrySetResult(true))
                {
                    Set();
                }
            }
            else
            {
                m_signaled = 1;
            }
        }
    }
}