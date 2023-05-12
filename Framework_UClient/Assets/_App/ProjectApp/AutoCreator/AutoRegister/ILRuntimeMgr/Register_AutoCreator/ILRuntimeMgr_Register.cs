/****************************************************
    文件: ILRuntimeMgr_Register.cs
    作者: Clear
    日期: #CreateTime#
    类型: 框架自动创建(请勿修改)
    功能: ILRuntime自动注册
* ****************************************************/
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using FutureCore;

namespace ProjectApp
{
    public static partial class ILRuntimeMgr_Register
    {
        public static void RegisterAll(AppDomain appDomain)
        {
            appDomain.RegisterCrossBindingAdaptor(new BaseCtrlAdapter());
            appDomain.RegisterCrossBindingAdaptor(new BaseModelAdapter());
            appDomain.RegisterCrossBindingAdaptor(new BaseUICtrlAdapter());
            appDomain.RegisterCrossBindingAdaptor(new BaseUIAdapter());

        }

    }
}