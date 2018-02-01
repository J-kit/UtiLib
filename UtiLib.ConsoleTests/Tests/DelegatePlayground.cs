using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using UtiLib.Delegates;

namespace UtiLib.ConsoleTests.Tests
{
    /// <summary>
    /// Please ignore this
    /// It's just for testing
    /// </summary>
    public class DelegatePlayground
    {
        public static void CreateDelegate()
        {
            var methodDict = new Dictionary<string, ExtendedMethodInfo>();

            var obj = new DummyDelegateClass();
            var oType = obj.GetType();
            var methods = oType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var instanceExpression = Expression.Constant(obj);

            foreach (var method in methods)
            {
                var mParams = method.GetParameters();

                methodDict[method.Name] = new ExtendedMethodInfo
                {
                    Method = method,
                    Instance = obj,
                    Delegate = DelegateConverter.Convert(method, instanceExpression),
                    Params = mParams,
                    ReturnType = method.ReturnType,
                    ReturnParameter = method.ReturnParameter
                };
            }

            methodDict["DoStuff"].Call();
            Debugger.Break();
        }
    }

    public class DummyDelegateClass
    {
        private int _counter;
        public int Counter => _counter++;

        public void DoStuff()
        {
            Console.WriteLine("Did Stuff!");
        }

        public void WriteStuff(string input)
        {
            Console.WriteLine(input);
        }

        private void YouCantSeeMe()
        {
            Console.WriteLine("Oh, you found me :/");
        }

        private void DontTouchMe(string input)
        {
            Console.WriteLine($"Eww, {input}");
        }
    }
}