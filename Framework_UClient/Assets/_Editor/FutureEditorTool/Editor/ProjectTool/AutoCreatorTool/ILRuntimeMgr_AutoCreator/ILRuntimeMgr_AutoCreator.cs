/****************************************************
    文件: ILRuntimeMgr_AutoCreate.cs
    作者: Clear
    日期: #CreateTime#
    类型: 框架核心脚本(请勿修改)
    功能: ILRuntimeMgr_AutoCreator
*****************************************************/
using System;
using System.IO;
using FutureCore;
using UnityEngine;

namespace FutureEditor
{
    public static class ILRuntimeMgr_AutoCreator
    {
        private static string autoRegisterPath = UnityEditorPathConst.AutoRegisterPath+ "/ILRuntimeMgr";
        private static string ILRuntimeMgr_RegisterClass = @"/****************************************************
    文件: ILRuntimeMgr_Register.cs
    作者: Clear
    日期: #CreateTime#
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
//ReplaceBinder
        }

    }
}";
    
        private static string tempStr;
        public static void CreatorAdapter()
        {
            Debug.Log("[ILRuntimeMgr_AutoCreator]开始注册热更适配器");


            if (!Directory.Exists(autoRegisterPath + "/Adapter_AutoCreator"))
            {
                Directory.CreateDirectory(autoRegisterPath + "/Adapter_AutoCreator");
            }
            tempStr = string.Empty;
            CreatorTypeAdapter(typeof(BaseCtrl));
            CreatorTypeAdapter(typeof(BaseModel));
            CreatorTypeAdapter(typeof(BaseUICtrl));
            CreatorTypeAdapter(typeof(BaseUI));

            if (!Directory.Exists(autoRegisterPath + "/Register_AutoCreator"))
            {
                Directory.CreateDirectory(autoRegisterPath + "/Register_AutoCreator");
            }
            CreatorRegister();
            
            
            Debug.Log("[ILRuntimeMgr_AutoCreator]注册热更适配器完成");
        }

        private static void CreatorRegister()
        {
            string path = autoRegisterPath + "/Register_AutoCreator/ILRuntimeMgr_Register.cs";
            string classVal = ILRuntimeMgr_RegisterClass.Replace("//ReplaceBinder", tempStr);  
            File.WriteAllText(path, classVal, System.Text.Encoding.UTF8);
            Debug.Log($"[ILRuntimeMgr_AutoCreator]生成注册器完成");
        }

        private static void CreatorTypeAdapter(Type type)
        {
            string path = autoRegisterPath + "/Adapter_AutoCreator/" + type.Name+ "Adapter.cs";
            
            string classVal = ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "FutureCore");
            tempStr += $"            appDomain.RegisterCrossBindingAdaptor(new {type.Name}Adapter());\r\n";
            File.WriteAllText(path,classVal,System.Text.Encoding.UTF8);
            Debug.Log($"[ILRuntimeMgr_AutoCreator]注册{type.Name}适配器完成");
        }


    }
}