/****************************************************
    文件: ModuleMgrRegister.cs
    作者: Clear
    日期: 2022/5/3 14:25:7
    类型: 自动创建
    功能: 模块数据注册
*****************************************************/
using FutureCore;
using System;
using UnityEngine;

namespace ProjectApp
{
    public static partial class ModuleMgrRegister 
    {
        public static void RegisterCommom()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
         
            moduleMgr.AddModel(ModelConst.LoadModel, new LoadModel()); 
            moduleMgr.AddUIType(UIConst.LoadUI, typeof(LoadUI));
            moduleMgr.AddCtrl(CtrlConst.LoadCtrl, new LoadCtrl());      
            moduleMgr.AddUICtrl(UICtrlConst.LoadUICtrl, new LoadUICtrl());


        }


    }
}