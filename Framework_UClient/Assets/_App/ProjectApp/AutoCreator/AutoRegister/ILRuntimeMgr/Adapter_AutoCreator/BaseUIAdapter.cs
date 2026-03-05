using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ProjectApp
{   
    public class BaseUIAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FutureCore.BaseUI);
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

        public class Adapter : FutureCore.BaseUI, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mAssignment_0 = new CrossBindingMethodInfo("Assignment");
            CrossBindingMethodInfo mUnAssignment_1 = new CrossBindingMethodInfo("UnAssignment");
            CrossBindingMethodInfo mOnNew_2 = new CrossBindingMethodInfo("OnNew");
            CrossBindingMethodInfo<FutureCore.UIInfo> mSetUIInfo_3 = new CrossBindingMethodInfo<FutureCore.UIInfo>("SetUIInfo");
            CrossBindingMethodInfo mOnInit_4 = new CrossBindingMethodInfo("OnInit");
            CrossBindingMethodInfo mOnBind_5 = new CrossBindingMethodInfo("OnBind");
            CrossBindingMethodInfo<System.Object> mOnOpenBefore_6 = new CrossBindingMethodInfo<System.Object>("OnOpenBefore");
            CrossBindingMethodInfo<System.Object> mOnOpen_7 = new CrossBindingMethodInfo<System.Object>("OnOpen");
            CrossBindingMethodInfo mAddListener_8 = new CrossBindingMethodInfo("AddListener");
            CrossBindingMethodInfo mOpenUIAnimEnd_9 = new CrossBindingMethodInfo("OpenUIAnimEnd");
            CrossBindingMethodInfo<System.Object> mOnDisplay_10 = new CrossBindingMethodInfo<System.Object>("OnDisplay");
            CrossBindingMethodInfo mOnUpdate_11 = new CrossBindingMethodInfo("OnUpdate");
            CrossBindingMethodInfo mOnHide_12 = new CrossBindingMethodInfo("OnHide");
            CrossBindingMethodInfo mCloseUIAnimEnd_13 = new CrossBindingMethodInfo("CloseUIAnimEnd");
            CrossBindingMethodInfo mRemoveListener_14 = new CrossBindingMethodInfo("RemoveListener");
            CrossBindingMethodInfo mOnClose_15 = new CrossBindingMethodInfo("OnClose");
            CrossBindingMethodInfo mOnDestroy_16 = new CrossBindingMethodInfo("OnDestroy");

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

            protected override void SetUIInfo(FutureCore.UIInfo uiInfo)
            {
                mSetUIInfo_3.Invoke(this.instance, uiInfo);
            }

            protected override void OnInit()
            {
                if (mOnInit_4.CheckShouldInvokeBase(this.instance))
                    base.OnInit();
                else
                    mOnInit_4.Invoke(this.instance);
            }

            protected override void OnBind()
            {
                if (mOnBind_5.CheckShouldInvokeBase(this.instance))
                    base.OnBind();
                else
                    mOnBind_5.Invoke(this.instance);
            }

            protected override void OnOpenBefore(System.Object args)
            {
                if (mOnOpenBefore_6.CheckShouldInvokeBase(this.instance))
                    base.OnOpenBefore(args);
                else
                    mOnOpenBefore_6.Invoke(this.instance, args);
            }

            protected override void OnOpen(System.Object args)
            {
                if (mOnOpen_7.CheckShouldInvokeBase(this.instance))
                    base.OnOpen(args);
                else
                    mOnOpen_7.Invoke(this.instance, args);
            }

            protected override void AddListener()
            {
                if (mAddListener_8.CheckShouldInvokeBase(this.instance))
                    base.AddListener();
                else
                    mAddListener_8.Invoke(this.instance);
            }

            protected override void OpenUIAnimEnd()
            {
                if (mOpenUIAnimEnd_9.CheckShouldInvokeBase(this.instance))
                    base.OpenUIAnimEnd();
                else
                    mOpenUIAnimEnd_9.Invoke(this.instance);
            }

            protected override void OnDisplay(System.Object arge)
            {
                if (mOnDisplay_10.CheckShouldInvokeBase(this.instance))
                    base.OnDisplay(arge);
                else
                    mOnDisplay_10.Invoke(this.instance, arge);
            }

            public override void OnUpdate()
            {
                if (mOnUpdate_11.CheckShouldInvokeBase(this.instance))
                    base.OnUpdate();
                else
                    mOnUpdate_11.Invoke(this.instance);
            }

            protected override void OnHide()
            {
                if (mOnHide_12.CheckShouldInvokeBase(this.instance))
                    base.OnHide();
                else
                    mOnHide_12.Invoke(this.instance);
            }

            protected override void CloseUIAnimEnd()
            {
                if (mCloseUIAnimEnd_13.CheckShouldInvokeBase(this.instance))
                    base.CloseUIAnimEnd();
                else
                    mCloseUIAnimEnd_13.Invoke(this.instance);
            }

            protected override void RemoveListener()
            {
                if (mRemoveListener_14.CheckShouldInvokeBase(this.instance))
                    base.RemoveListener();
                else
                    mRemoveListener_14.Invoke(this.instance);
            }

            protected override void OnClose()
            {
                if (mOnClose_15.CheckShouldInvokeBase(this.instance))
                    base.OnClose();
                else
                    mOnClose_15.Invoke(this.instance);
            }

            protected override void OnDestroy()
            {
                if (mOnDestroy_16.CheckShouldInvokeBase(this.instance))
                    base.OnDestroy();
                else
                    mOnDestroy_16.Invoke(this.instance);
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
