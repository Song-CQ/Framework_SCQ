using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace FutureCore
{   
    public class BaseUICtrlAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FutureCore.BaseUICtrl);
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

        public class Adapter : FutureCore.BaseUICtrl, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mOnInit_0 = new CrossBindingMethodInfo("OnInit");
            CrossBindingFunctionInfo<System.String, System.UInt32> mGetOpenUIMsg_1 = new CrossBindingFunctionInfo<System.String, System.UInt32>("GetOpenUIMsg");
            CrossBindingFunctionInfo<System.String, System.UInt32> mGetCloseUIMsg_2 = new CrossBindingFunctionInfo<System.String, System.UInt32>("GetCloseUIMsg");
            CrossBindingMethodInfo<System.Object> mOpenUI_3 = new CrossBindingMethodInfo<System.Object>("OpenUI");
            CrossBindingMethodInfo<System.Object> mCloseUI_4 = new CrossBindingMethodInfo<System.Object>("CloseUI");
            CrossBindingMethodInfo mOnDispose_5 = new CrossBindingMethodInfo("OnDispose");
            CrossBindingMethodInfo mInit_6 = new CrossBindingMethodInfo("Init");
            CrossBindingMethodInfo mStartUp_7 = new CrossBindingMethodInfo("StartUp");
            CrossBindingMethodInfo mGameStart_8 = new CrossBindingMethodInfo("GameStart");
            CrossBindingMethodInfo mDispose_9 = new CrossBindingMethodInfo("Dispose");
            CrossBindingMethodInfo mAssignment_10 = new CrossBindingMethodInfo("Assignment");
            CrossBindingMethodInfo mUnAssignment_11 = new CrossBindingMethodInfo("UnAssignment");
            CrossBindingMethodInfo mOnNew_12 = new CrossBindingMethodInfo("OnNew");
            CrossBindingMethodInfo mOnStartUp_13 = new CrossBindingMethodInfo("OnStartUp");
            CrossBindingMethodInfo mOnReadData_14 = new CrossBindingMethodInfo("OnReadData");
            CrossBindingMethodInfo mOnGameStart_15 = new CrossBindingMethodInfo("OnGameStart");
            CrossBindingMethodInfo mAddListener_16 = new CrossBindingMethodInfo("AddListener");
            CrossBindingMethodInfo mRemoveListener_17 = new CrossBindingMethodInfo("RemoveListener");

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

            protected override void OnInit()
            {
                if (mOnInit_0.CheckShouldInvokeBase(this.instance))
                    base.OnInit();
                else
                    mOnInit_0.Invoke(this.instance);
            }

            public override System.UInt32 GetOpenUIMsg(System.String uiName)
            {
                if (mGetOpenUIMsg_1.CheckShouldInvokeBase(this.instance))
                    return base.GetOpenUIMsg(uiName);
                else
                    return mGetOpenUIMsg_1.Invoke(this.instance, uiName);
            }

            public override System.UInt32 GetCloseUIMsg(System.String uiName)
            {
                if (mGetCloseUIMsg_2.CheckShouldInvokeBase(this.instance))
                    return base.GetCloseUIMsg(uiName);
                else
                    return mGetCloseUIMsg_2.Invoke(this.instance, uiName);
            }

            public override void OpenUI(System.Object args)
            {
                mOpenUI_3.Invoke(this.instance, args);
            }

            public override void CloseUI(System.Object args)
            {
                mCloseUI_4.Invoke(this.instance, args);
            }

            protected override void OnDispose()
            {
                if (mOnDispose_5.CheckShouldInvokeBase(this.instance))
                    base.OnDispose();
                else
                    mOnDispose_5.Invoke(this.instance);
            }

            public override void Init()
            {
                if (mInit_6.CheckShouldInvokeBase(this.instance))
                    base.Init();
                else
                    mInit_6.Invoke(this.instance);
            }

            public override void StartUp()
            {
                if (mStartUp_7.CheckShouldInvokeBase(this.instance))
                    base.StartUp();
                else
                    mStartUp_7.Invoke(this.instance);
            }

            public override void GameStart()
            {
                if (mGameStart_8.CheckShouldInvokeBase(this.instance))
                    base.GameStart();
                else
                    mGameStart_8.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDispose_9.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_9.Invoke(this.instance);
            }

            protected override void Assignment()
            {
                if (mAssignment_10.CheckShouldInvokeBase(this.instance))
                    base.Assignment();
                else
                    mAssignment_10.Invoke(this.instance);
            }

            protected override void UnAssignment()
            {
                if (mUnAssignment_11.CheckShouldInvokeBase(this.instance))
                    base.UnAssignment();
                else
                    mUnAssignment_11.Invoke(this.instance);
            }

            protected override void OnNew()
            {
                if (mOnNew_12.CheckShouldInvokeBase(this.instance))
                    base.OnNew();
                else
                    mOnNew_12.Invoke(this.instance);
            }

            protected override void OnStartUp()
            {
                if (mOnStartUp_13.CheckShouldInvokeBase(this.instance))
                    base.OnStartUp();
                else
                    mOnStartUp_13.Invoke(this.instance);
            }

            protected override void OnReadData()
            {
                if (mOnReadData_14.CheckShouldInvokeBase(this.instance))
                    base.OnReadData();
                else
                    mOnReadData_14.Invoke(this.instance);
            }

            protected override void OnGameStart()
            {
                if (mOnGameStart_15.CheckShouldInvokeBase(this.instance))
                    base.OnGameStart();
                else
                    mOnGameStart_15.Invoke(this.instance);
            }

            protected override void AddListener()
            {
                if (mAddListener_16.CheckShouldInvokeBase(this.instance))
                    base.AddListener();
                else
                    mAddListener_16.Invoke(this.instance);
            }

            protected override void RemoveListener()
            {
                if (mRemoveListener_17.CheckShouldInvokeBase(this.instance))
                    base.RemoveListener();
                else
                    mRemoveListener_17.Invoke(this.instance);
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
