using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Func<T, bool> func)
        {
            foreach (var e in source)
            {
                var flag = func(e);
                if (flag)
                {
                    yield return e;
                }
            }
        }

        public static IEnumerable<TX> RForEach<T, TX>(this IEnumerable<T> source, Func<T, TX> func)
        {
            foreach (var e in source)
            {
                yield return func(e);
            }
        }

        /// <summary>
        /// Iterates through an IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int ForEach<T>(this IEnumerable<T> input, Action<T> action)
        {
            int iterations = 0;
            if (input == null) return iterations;
            foreach (var put in input)
            {
                if (!put.Equals(default(T)))
                {
                    action(put);
                    iterations++;
                }
            }
            return iterations;
        }

        /// <summary>
        /// Swallows every exception that occurs in the foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        public static void ForEachSwallow<T>(this IEnumerable<T> input, Action<T> action, Action<Exception> onException = null)
        {
            if (input == null) return;
            foreach (var put in input)
            {
                try
                {
                    if (!put.Equals(default(T)))
                        action(put);
                }
                catch (Exception ex)
                {
                    onException?.Invoke(ex);
                }
            }
        }
    }
}