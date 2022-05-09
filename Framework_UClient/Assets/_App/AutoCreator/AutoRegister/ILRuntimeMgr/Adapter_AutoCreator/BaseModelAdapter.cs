using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace FutureCore
{   
    public class BaseModelAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FutureCore.BaseModel);
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

        public class Adapter : FutureCore.BaseModel, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mAssignment_0 = new CrossBindingMethodInfo("Assignment");
            CrossBindingMethodInfo mUnAssignment_1 = new CrossBindingMethodInfo("UnAssignment");
            CrossBindingMethodInfo mOnNew_2 = new CrossBindingMethodInfo("OnNew");
            CrossBindingMethodInfo mOnInit_3 = new CrossBindingMethodInfo("OnInit");
            CrossBindingMethodInfo mOnStartUp_4 = new CrossBindingMethodInfo("OnStartUp");
            CrossBindingMethodInfo mOnReadData_5 = new CrossBindingMethodInfo("OnReadData");
            CrossBindingMethodInfo mOnGameStart_6 = new CrossBindingMethodInfo("OnGameStart");
            CrossBindingMethodInfo mOnReset_7 = new CrossBindingMethodInfo("OnReset");
            CrossBindingMethodInfo mOnDispose_8 = new CrossBindingMethodInfo("OnDispose");
            CrossBindingMethodInfo mAddListener_9 = new CrossBindingMethodInfo("AddListener");
            CrossBindingMethodInfo mRemoveListener_10 = new CrossBindingMethodInfo("RemoveListener");
            CrossBindingMethodInfo mWriteLocalStorage_11 = new CrossBindingMethodInfo("WriteLocalStorage");

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

            protected override void Assignment()
            {
                if (mAssignment_0.CheckShouldInvokeBase(this.instance))
                    base.Assignment();
                else
                    mAssignment_0.Invoke(this.instance);
            }

            protected override void UnAssignment()
            {
                if (mUnAssignment_1.CheckShouldInvokeBase(this.instance))
                    base.UnAssignment();
                else
                    mUnAssignment_1.Invoke(this.instance);
            }

            protected override void OnNew()
            {
                if (mOnNew_2.CheckShouldInvokeBase(this.instance))
                    base.OnNew();
                else
                    mOnNew_2.Invoke(this.instance);
            }

            protected override void OnInit()
            {
                mOnInit_3.Invoke(this.instance);
            }

            protected override void OnStartUp()
            {
                if (mOnStartUp_4.CheckShouldInvokeBase(this.instance))
                    base.OnStartUp();
                else
                    mOnStartUp_4.Invoke(this.instance);
            }

            protected override void OnReadData()
            {
                if (mOnReadData_5.CheckShouldInvokeBase(this.instance))
                    base.OnReadData();
                else
                    mOnReadData_5.Invoke(this.instance);
            }

            protected override void OnGameStart()
            {
                if (mOnGameStart_6.CheckShouldInvokeBase(this.instance))
                    base.OnGameStart();
                else
                    mOnGameStart_6.Invoke(this.instance);
            }

            protected override void OnReset()
            {
                mOnReset_7.Invoke(this.instance);
            }

            protected override void OnDispose()
            {
                mOnDispose_8.Invoke(this.instance);
            }

            protected override void AddListener()
            {
                if (mAddListener_9.CheckShouldInvokeBase(this.instance))
                    base.AddListener();
                else
                    mAddListener_9.Invoke(this.instance);
            }

            protected override void RemoveListener()
            {
                if (mRemoveListener_10.CheckShouldInvokeBase(this.instance))
                    base.RemoveListener();
                else
                    mRemoveListener_10.Invoke(this.instance);
            }

            protected override void WriteLocalStorage()
            {
                if (mWriteLocalStorage_11.CheckShouldInvokeBase(this.instance))
                    base.WriteLocalStorage();
                else
                    mWriteLocalStorage_11.Invoke(this.instance);
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
