/****************************************************
    文件: ILRuntimeMgr_Register.cs
    作者: Clear
    日期: 2024/7/19 17:47:57
    类型: 通用ILRuntimeMgr注册
    功能: ILRuntime自动注册
*****************************************************/
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System;

namespace ProjectApp
{
    public static partial class  ILRuntimeMgr_Register
    {

        public static void RegisterAll(ILRuntime.Runtime.Enviorment.AppDomain appDomain)
        {
            RegisterFunctionDelegate(appDomain);
            RegisterCommon(appDomain);
            RegisterProject(appDomain);
        }

        private static void RegisterCommon(ILRuntime.Runtime.Enviorment.AppDomain appDomain)
        {
            

            //appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            //appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            //appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

        }

        public static void RegisterFunctionDelegate(ILRuntime.Runtime.Enviorment.AppDomain appDomain)
        {
            
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

        }



    }
}