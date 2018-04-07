using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public static class MemberInfoExtensions
    {
        public static bool HasCustomAttribute<T>(this PropertyInfo pInfo) where T : Attribute
        {
            return pInfo.GetCustomAttributes<T>().Any();
        }
    }
}