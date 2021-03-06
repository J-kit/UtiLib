﻿using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    public static class LinqExtensions
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
            if (input == null)
                return iterations;
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

        public static int ForEach<T>(this IEnumerable input, Action<T> action)
        {
            int iterations = 0;
            if (input == null)
                return iterations;
            foreach (var put in input)
            {
                if (!put.Equals(default(T)))
                {
                    action((T)put);
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
            if (input == null)
                return;
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

        /// <summary>
        /// Combines .Select(x).ToArray()
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult[] ToArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToArray();
        }

        public static bool If<T>(this T input, Func<T, bool> func)
        {
            return func(input);
        }

        public static bool IsOneMatch<T>(this IEnumerable<T> input, IEnumerable<T> matchlist)
        {
            return input.Any(matchlist.Contains);
        }
    }

    public static class QueueExtensions
    {
        public static List<T> DequeueRange<T>(this ConcurrentQueue<T> queue)
        {
            var result = new List<T>();
            while (queue.TryDequeue(out var element))
            {
                result.Add(element);
            }

            return result;
        }
    }
}
