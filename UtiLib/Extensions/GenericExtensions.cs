// ReSharper disable once CheckNamespace
namespace System
{
    public static class GenericExtensions
    {
        public static T ExecuteOn<T>(this T input, Action<T> action, Predicate<T> condition = null)
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
    }
}