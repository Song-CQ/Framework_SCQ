using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ProjectApp
{   
    public class BaseSystemAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FutureCore.BaseSystem);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : FutureCore.BaseSystem, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mInit_0 = new CrossBindingMethodInfo("Init");
            CrossBindingMethodInfo mStart_1 = new CrossBindingMethodInfo("Start");
            CrossBindingMethodInfo mShutdown_2 = new CrossBindingMethodInfo("Shutdown");
            CrossBindingMethodInfo mDisplay_3 = new CrossBindingMethodInfo("Display");

            bool isInvokingToString;
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void Init()
            {
                if (mInit_0.CheckShouldInvokeBase(this.instance))
                    base.Init();
                else
                    mInit_0.Invoke(this.instance);
            }

            public override void Start()
            {
                if (mStart_1.CheckShouldInvokeBase(this.instance))
                    base.Start();
                else
                    mStart_1.Invoke(this.instance);
            }

            public override void Shutdown()
            {
                if (mShutdown_2.CheckShouldInvokeBase(this.instance))
                    base.Shutdown();
                else
                    mShutdown_2.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDisplay_3.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDisplay_3.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    if (!isInvokingToString)
                    {
                        isInvokingToString = true;
                        string res = instance.ToString();
                        isInvokingToString = false;
                        return res;
                    }
                    else
                        return instance.Type.FullName;
                }
                else
                    return instance.Type.FullName;
            }
        }
    }

    public class BaseMgrAdapter<T> : CrossBindingAdaptor  where T : FutureCore.BaseMgr<T>, new()
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FutureCore.BaseMgr<T>);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : FutureCore.BaseMgr<T>, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mInit_0 = new CrossBindingMethodInfo("Init");
            CrossBindingMethodInfo mStartUp_1 = new CrossBindingMethodInfo("StartUp");
            CrossBindingMethodInfo mDisposeBefore_2 = new CrossBindingMethodInfo("DisposeBefore");
            CrossBindingMethodInfo mDisplay_3 = new CrossBindingMethodInfo("Display");
            CrossBindingMethodInfo mNew_4 = new CrossBindingMethodInfo("New");

            bool isInvokingToString;
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void Init()
            {
                if (mInit_0.CheckShouldInvokeBase(this.instance))
                    base.Init();
                else
                    mInit_0.Invoke(this.instance);
            }

            public override void StartUp()
            {
                if (mStartUp_1.CheckShouldInvokeBase(this.instance))
                    base.StartUp();
                else
                    mStartUp_1.Invoke(this.instance);
            }

            protected override void New()
            {
                if (mNew_4.CheckShouldInvokeBase(this.instance))
                    base.New();
                else
                    mNew_4.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDisplay_3.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDisplay_3.Invoke(this.instance);
            }

            public override void DisposeBefore()
            {
                if (mDisposeBefore_2.CheckShouldInvokeBase(this.instance))
                    base.DisposeBefore();
                else
                    mDisposeBefore_2.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    if (!isInvokingToString)
                    {
                        isInvokingToString = true;
                        string res = instance.ToString();
                        isInvokingToString = false;
                        return res;
                    }
                    else
                        return instance.Type.FullName;
                }
                else
                    return instance.Type.FullName;
            }
        }
    }


}
