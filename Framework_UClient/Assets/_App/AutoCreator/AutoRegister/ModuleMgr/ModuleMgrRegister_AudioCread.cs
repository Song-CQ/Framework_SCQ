/****************************************************
    文件: ModuleMgrRegister.cs
    作者: Clear
    日期: 2022/2/3 19:22:0
    类型: 自动创建
    功能: 模块数据注册
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public static class ModuleMgrRegister 
    {
        public static void AutoRegisterModel()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddModel(ModelConst.MainModel,new MainModel());

        }

        public static void AutoRegisterUIType()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUIType(UIConst.MainUI,typeof(MainUI));

        }

        public static void AutoRegisterCtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddCtrl(CtrlConst.MainCtrl,new MainCtrl());

        }

        public static void AutoRegisterUICtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUICtrl(UICtrlConst.MainUICtrl,new MainUICtrl());

        }

    }
}