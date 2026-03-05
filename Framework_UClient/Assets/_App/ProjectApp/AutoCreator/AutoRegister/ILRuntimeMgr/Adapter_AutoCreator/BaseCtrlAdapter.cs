using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ProjectApp
{   
    public class BaseCtrlAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FutureCore.BaseCtrl);
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

        public class Adapter : FutureCore.BaseCtrl, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mInit_0 = new CrossBindingMethodInfo("Init");
            CrossBindingMethodInfo mStartUp_1 = new CrossBindingMethodInfo("StartUp");
            CrossBindingMethodInfo mGameStart_2 = new CrossBindingMethodInfo("GameStart");
            CrossBindingMethodInfo mDispose_3 = new CrossBindingMethodInfo("Dispose");
            CrossBindingMethodInfo mAssignment_4 = new CrossBindingMethodInfo("Assignment");
            CrossBindingMethodInfo mUnAssignment_5 = new CrossBindingMethodInfo("UnAssignment");
            CrossBindingMethodInfo mOnNew_6 = new CrossBindingMethodInfo("OnNew");
            CrossBindingMethodInfo mOnInit_7 = new CrossBindingMethodInfo("OnInit");
            CrossBindingMethodInfo mOnStartUp_8 = new CrossBindingMethodInfo("OnStartUp");
            CrossBindingMethodInfo mOnReadData_9 = new CrossBindingMethodInfo("OnReadData");
            CrossBindingMethodInfo mOnGameStart_10 = new CrossBindingMethodInfo("OnGameStart");
            CrossBindingMethodInfo mOnDispose_11 = new CrossBindingMethodInfo("OnDispose");
            CrossBindingMethodInfo mAddListener_12 = new CrossBindingMethodInfo("AddListener");
            CrossBindingMethodInfo mRemoveListener_13 = new CrossBindingMethodInfo("RemoveListener");

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

            public override void GameStart()
            {
                if (mGameStart_2.CheckShouldInvokeBase(this.instance))
                    base.GameStart();
                else
                    mGameStart_2.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDispose_3.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_3.Invoke(this.instance);
            }

            protected override void Assignment()
            {
                if (mAssignment_4.CheckShouldInvokeBase(this.instance))
                    base.Assignment();
                else
                    mAssignment_4.Invoke(this.instance);
            }

            protected override void UnAssignment()
            {
                if (mUnAssignment_5.CheckShouldInvokeBase(this.instance))
                    base.UnAssignment();
                else
                    mUnAssignment_5.Invoke(this.instance);
            }

            protected override void OnNew()
            {
                if (mOnNew_6.CheckShouldInvokeBase(this.instance))
                    base.OnNew();
                else
                    mOnNew_6.Invoke(this.instance);
            }

            protected override void OnInit()
            {
                mOnInit_7.Invoke(this.instance);
            }

            protected override void OnStartUp()
            {
                if (mOnStartUp_8.CheckShouldInvokeBase(this.instance))
                    base.OnStartUp();
                else
                    mOnStartUp_8.Invoke(this.instance);
            }

            protected override void OnReadData()
            {
                if (mOnReadData_9.CheckShouldInvokeBase(this.instance))
                    base.OnReadData();
                else
                    mOnReadData_9.Invoke(this.instance);
            }

            protected override void OnGameStart()
            {
                if (mOnGameStart_10.CheckShouldInvokeBase(this.instance))
                    base.OnGameStart();
                else
                    mOnGameStart_10.Invoke(this.instance);
            }

            protected override void OnDispose()
            {
                mOnDispose_11.Invoke(this.instance);
            }

            protected override void AddListener()
            {
                if (mAddListener_12.CheckShouldInvokeBase(this.instance))
                    base.AddListener();
                else
                    mAddListener_12.Invoke(this.instance);
            }

            protected override void RemoveListener()
            {
                if (mRemoveListener_13.CheckShouldInvokeBase(this.instance))
                    base.RemoveListener();
                else
                    mRemoveListener_13.Invoke(this.instance);
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
