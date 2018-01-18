using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

    public class ExtendedMethodInfo
    {
        public MethodInfo Method { get; set; }
        public ParameterInfo[] Params { get; set; }

        public Type ReturnType { get; set; }
        public ParameterInfo ReturnParameter { get; set; }

        public Delegate Delegate { get; set; }
        public object Instance { get; set; }

        public Func<object[], object> FuncDelegate => _funcDelegate ?? (_funcDelegate = GenerateFunc());

        private Func<object[], object> _funcDelegate;

        private Func<object[], object> GenerateFunc()
        {
            if (Method.ReturnType == typeof(void))
            {
                var cDele = (Action<object[]>)Delegate;
                return (x =>
                {
                    cDele(x);
                    return null;
                });
            }
            return (Func<object[], object>)Delegate;
        }

        public object Call(params object[] input)
        {
            return FuncDelegate(input);
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