using System;
using System.Threading.Tasks;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<int> ForEachAsync<T>(this IAsyncEnumerable<T> input, Action<T> action)
        {
            var iterations = 0;
            if (input == null) return iterations;

            using (var enumerator = input.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    var val = enumerator.Current;
                    if (!val.Equals(default(T)))
                    {
                        action(val);
                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public static async Task<int> ForEachAsync<T>(this IAsyncEnumerable<T> input, Func<T, Task> function)
        {
            var iterations = 0;
            if (input == null) return iterations;

            using (var enumerator = input.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    var val = enumerator.Current;
                    if (!val.Equals(default(T)))
                    {
                        await function(val);
                        iterations++;
                    }
                }
            }

            return iterations;
        }
    }
}