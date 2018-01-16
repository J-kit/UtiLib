using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class QueueExtensions
    {
        public static bool TryDequeue<T>(this Queue<T> dstQueue, out T result)
        {
            if (dstQueue.Count <= 0)
            {
                result = default(T);
                return false;
            }

            result = dstQueue.Dequeue();
            return true;
        }
    }
}