using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets us all methods from a class which match our needs
        /// </summary>
        /// <param name="classToSearchIn">The parent class where the methods are located</param>
        /// <param name="matchInputParams">null means any</param>
        /// <param name="outputparam">null means any</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethodsFromType(this Type classToSearchIn, Type[] matchInputParams = null, Type outputparam = null)
        {
            IEnumerable<MethodInfo> methodArray = classToSearchIn.UnderlyingSystemType.GetMethods();

            if (matchInputParams != null)
            {
                methodArray = methodArray.Where(m => m.GetParameters().Length == matchInputParams.Length);

                if (matchInputParams.Length > 0)
                {
                    methodArray = methodArray
                        .Where(a => a.GetParameters()
                            .Where((info, i) => info.ParameterType == matchInputParams[i])
                            .Any());
                }
            }

            if (outputparam != null)
                methodArray = methodArray.Where(m => m.ReturnParameter?.ParameterType == outputparam);

            return methodArray;
        }

        /// <summary>
        /// Gets us an unique Identifier for <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ulong GetMetaDataToken<T>() => GetMetaDataToken(typeof(T));

        /// <summary>
        /// Gets us an unique Identifier for <see cref="input"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ulong GetMetaDataToken(this object input) => GetMetaDataToken(input.GetType());

        /// <summary>
        /// Gets us an unique Identifier for <see cref="input"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ulong GetMetaDataToken(this Type input)
        {
            return unchecked(((ulong)(uint)input.Module.ModuleVersionId.GetHashCode() << 32) | (ulong)(uint)input.MetadataToken);
        }
    }
}