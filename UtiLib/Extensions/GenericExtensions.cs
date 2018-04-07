using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Allows to execute any command on any object (designed for oneliners)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T Modify<T>(this T input, Action<T> action, Predicate<T> condition = null)
        {
            if (condition == null || condition.Invoke(input))
                action(input);

            return input;
        }

        public static void OnSuccess<T>(this Tuple<bool, T> tulpe, Action<T> doThat)
        {
            if (tulpe.Item1)
            {
                doThat(tulpe.Item2);
            }
        }

        public static TX OnSuccess<T, TX>(this Tuple<bool, T> tulpe, Func<T, TX> doThat)
        {
            if (tulpe.Item1)
            {
                return doThat(tulpe.Item2);
            }
            return default(TX);
        }

        /// <summary>
        /// Fills up the given array with <see cref="value"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Propagate<T>(this T[] dArray, T value)
        {
            for (int i = 0; i < dArray.Length; i++)
            {
                dArray[i] = value;
            }

            return dArray;
        }

        public static T[] Propagate<T>(this T[] dArray, Func<int, T> func)
        {
            for (int i = 0; i < dArray.Length; i++)
            {
                dArray[i] = func(i);
            }
            return dArray;
        }

        public static List<T> ToList<T>(this T[] input)
        {
            return new List<T>(input);
        }

        public static bool IsGenericIEnumerable(this Type oType)
        {
            return oType.IsGeneticTypeOf(typeof(IEnumerable<>));
        }

        public static bool IsGenericList(this Type oType)
        {
            return oType.IsGeneticTypeOf(typeof(List<>));
        }

        public static bool IsGeneticTypeOf(this Type oType, Type of)
        {
            return (oType.IsGenericType && oType.GetGenericTypeDefinition() == of);
        }

        public static bool TryGetArrayType(this Type oType, out Type typeOf)
        {
            if (oType.IsArray)
            {
                typeOf = oType.GetElementType();
                return true;
            }
            typeOf = default;
            return false;
        }
    }
}