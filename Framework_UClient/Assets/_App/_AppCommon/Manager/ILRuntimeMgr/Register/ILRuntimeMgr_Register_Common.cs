/****************************************************
    文件: ILRuntimeMgr_Register.cs
    作者: Clear
    日期: 2024/7/19 17:47:57
    类型: 通用ILRuntimeMgr注册
    功能: ILRuntime自动注册
*****************************************************/
using UnityEngine;
using ILRuntime.Runtime.Enviorment;


namespace ProjectApp
{
    public static partial class  ILRuntimeMgr_Register
    {

        public static void RegisterAll(AppDomain appDomain)
        {
            RegisterCommon(appDomain);
            RegisterProject(appDomain);
        }

        private static void RegisterCommon(AppDomain appDomain)
        {
            

            //appDomain.RegisterCrossBindingAdaptor(new BaseMgrAdapter());

            //appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            //appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            //appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

        }

    }
}