/****************************************************
    文件：ModuleMgrRegister.cs
	作者：Clear
    日期：2022/1/26 15:5:53
    类型: 自动创建
	功能：模块数据注册
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
            moduleMgr.AddModel("Main",new MainModel());
        }

        public static void AutoRegisterUIType()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUIType("Main", typeof(MainUI));
        }

        public static void AutoRegisterCtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddCtrl("Main",new MainCtrl());
        }

        public static void AutoRegisterUICtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddCtrl("Main", new MainUICtrl());
        }

    }
}