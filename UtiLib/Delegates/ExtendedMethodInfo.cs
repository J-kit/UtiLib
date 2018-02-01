using System;
using System.Reflection;

namespace UtiLib.Delegates
{
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
}