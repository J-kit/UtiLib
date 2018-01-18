﻿using System;

namespace UtiLib.Delegates
{
    public class FuncConvert
    {
        /// <summary>
        /// Converts any generic Func to the given return type
        /// </summary>
        /// <param name="sourceDelegate">Should be Func<TX,...></param>
        /// <returns></returns>
        public static Func<object[], object> Convert(Delegate sourceDelegate)
        {
            if (sourceDelegate.Method.ReturnType == typeof(void))
                throw new Exception("Invalid source delegate Type");

            return (Func<object[], object>)DelegateConverter.Convert(sourceDelegate);
        }

        /// <summary>
        /// Converts any generic Func to the given return type
        /// </summary>
        /// <typeparam name="TReturn">The required return type</typeparam>
        /// <param name="sourceDelegate">Should be Func<TX,...></param>
        /// <returns></returns>
        public static Func<object[], TReturn> Convert<TReturn>(Delegate sourceDelegate)
        {
            if (sourceDelegate.Method.ReturnType == typeof(void))
                throw new Exception("Invalid source delegate Type");

            return (Func<object[], TReturn>)DelegateConverter.Convert(sourceDelegate, typeof(TReturn));
        }
    }
}