using System.Collections.Generic;
using System.Threading.Tasks;
using UtiLib.Shared.Interfaces;

// ReSharper disable once CheckNamespace
namespace System
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

        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> input)
        {
            return (await input.ToListAsync()).ToArray();
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> input)
        {
            var tList = new List<T>();
            await input.ForEachAsync(x => tList.Add(x));

            return tList;
        }
    }
}