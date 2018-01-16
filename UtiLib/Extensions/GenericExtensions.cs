using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Extensions
{
    public static class GenericExtensions
    {
        public static T ExecuteOn<T>(this T input, Action<T> action, Predicate<T> condition = null)
        {
            if (condition == null || condition.Invoke(input))
                action(input);

            return input;
        }
    }
}