using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class QueueExtensions
    {
        public static bool TryDequeue<T>(this Queue<T> dstQueue, out T result)
        {
            if (dstQueue.Count <= 0)
            {
                result = default;
                return false;
            }

            result = dstQueue.Dequeue();
            return true;
        }
    }
}