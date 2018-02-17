using System;
using System.Reflection;

using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

using UtiLib.Delegates;

namespace UtiLib.IO
{
    internal class ImplementationWrappingProxy<TInterface> : RealProxy where TInterface : class
    {
        private readonly Dictionary<string, ExtendedMethodInfo> _methodDict;
        private readonly object _instance;

        public ImplementationWrappingProxy(object instance) : base(typeof(TInterface))
        {
            _methodDict = new Dictionary<string, ExtendedMethodInfo>();
            _instance = instance;

            PrepareInterface(instance);
        }

        //private static IntPtr _defaultStub = GetDefaultStub();
        //private Object _tp;

        //private void FakeInitialize(Type classToProxy, IntPtr stub, Object stubData)
        //{
        //    //if (!classToProxy.IsMarshalByRef && !classToProxy.IsInterface)
        //    //{
        //    //    throw new ArgumentException(
        //    //        Environment.GetResourceString("Remoting_Proxy_ProxyTypeIsNotMBR"));
        //    //}
        //    Contract.EndContractBlock();

        //    if ((IntPtr)0 == stub)
        //    {
        //        Contract.Assert((IntPtr)0 != _defaultStub, "Default stub not set up");

        //        // The default stub checks for match of contexts defined by us
        //        stub = _defaultStub;
        //        // Start with a value of -1 because 0 is reserved for the default context
        //        stubData = new IntPtr(-1);
        //    }

        //    _tp = null;
        //    if (stubData == null)
        //    {
        //        throw new ArgumentNullException("stubdata");
        //    }
        //    _tp = RemotingServices.CreateTransparentProxy(this, classToProxy, stub, stubData);
        //    var rp = this as IRemotingTypeInfo;// RemotingProxy;
        //    if (rp != null)
        //    {
        //        _flags |= RealProxyFlags.RemotingProxy;
        //    }
        //}

        private void PrepareInterface(object instance)
        {
            var type = instance.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var instanceExpression = Expression.Constant(instance);

            foreach (var method in methods)
            {
                var mParams = method.GetParameters();

                _methodDict[method.Name] = new ExtendedMethodInfo
                {
                    Method = method,
                    Instance = instance,
                    Delegate = DelegateConverter.Convert(method, instanceExpression),
                    Params = mParams,
                    ReturnType = method.ReturnType,
                    ReturnParameter = method.ReturnParameter
                };
            }
        }

        public TInterface Create()
        {
            return base.GetTransparentProxy() as TInterface;
        }

        //typeof(TInterface)
        public override IMessage Invoke(IMessage msg)
        {
            if (!(msg is IMethodCallMessage mcm))
                return default;

            switch (mcm.MethodName)
            {
                case "GetType":
                    return new ReturnMessage(_instance.GetType(), (object[])null, 0, mcm.LogicalCallContext, mcm);

                case "GetHashCode":
                    return new ReturnMessage(_instance.GetHashCode(), (object[])null, 0, mcm.LogicalCallContext, mcm);
            }

            if (_methodDict.TryGetValue(mcm.MethodName, out var method))
            {
                return new ReturnMessage(method.Call(mcm.Args), (object[])null, 0, mcm.LogicalCallContext, mcm);
            }
            //if (mcm.MethodName.StartsWith("get_", "set_"))
            //{
            //    if (_methodDict.TryGetValue(mcm.MethodName, out var method))
            //    {
            //        return new ReturnMessage(method.Call(mcm.Args), (object[])null, 0, mcm.LogicalCallContext, mcm);
            //    }
            //}
            //else if (mcm.MethodName.StartsWith("add_"))
            //{
            //    //EventHandler_addEvent
            //    methodName = mcm.MethodName.Substring(4, mcm.MethodName.Length - 4);
            //    if (methodName == "PropertyChanged" && mcm.ArgCount == 1 && mcm.Args[0] is PropertyChangedEventHandler)
            //    {
            //        //Probchanged [TODO: MERGE]
            //        PropertyChanged += (PropertyChangedEventHandler)mcm.Args[0];
            //        return new ReturnMessage(null, (object[])null, 0, mcm.LogicalCallContext, mcm);
            //    }
            //}
            //else if (mcm.MethodName.StartsWith("remove_"))
            //{
            //    //EventHandler_removeEvent
            //    methodName = mcm.MethodName.Substring(7, mcm.MethodName.Length - 7);
            //    if (methodName == "PropertyChanged" && mcm.ArgCount == 1 && mcm.Args[0] is PropertyChangedEventHandler)
            //    {
            //        //Probchanged [TODO: MERGE]
            //        PropertyChanged -= (PropertyChangedEventHandler)mcm.Args[0];
            //        return new ReturnMessage(null, (object[])null, 0, mcm.LogicalCallContext, mcm);
            //    }
            //}
            //else
            //{
            //    System.Delegate cMethod;
            //    if (_methods.TryGetValue(mcm.MethodName, out cMethod))
            //    {
            //        //todo
            //    }
            //}

            return (IMessage)null;
        }
    }
}