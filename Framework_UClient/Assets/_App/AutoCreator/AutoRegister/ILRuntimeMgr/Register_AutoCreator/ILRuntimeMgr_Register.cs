/****************************************************
    文件: ILRuntimeMgr_Register.cs
    作者: Clear
    日期: 2022/5/9 16:48:30
    类型: 框架自动创建(请勿修改)
    功能: ILRuntime自动注册
* ****************************************************/
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
namespace FutureCore
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