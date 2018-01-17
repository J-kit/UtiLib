using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"> </typeparam>
        /// <param name="source"></param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,if the key is found; otherwise, <see cref="defaultValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="defaultValue"><see cref="value"/> will fall back to this value if the dictionary doesn't contain the specified key</param>
        /// <returns></returns>
        public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, out TValue value, TValue defaultValue)
        {
            if (source.TryGetValue(key, out value))
            {
                return true;
            }

            value = defaultValue;
            return false;
        }
    }
}