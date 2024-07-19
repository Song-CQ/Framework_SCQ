/****************************************************
    文件: GameLogicRegister.cs
    作者: Clear
    日期: 2024/7/16 14:10:29
    类型: 逻辑脚本
    功能: 本地游戏逻辑注册
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp.GameLogic
{
    public static class GameLogicRegister 
    {
        public static void Register()
        {
            GlobalMgr globalMgr = GlobalMgr.Instance;
            //// Mgr

            globalMgr.AddMgr(GameWorldMgr.Instance);
        }

        public static void RegisterData()
        {
            //Module
            RegisterModuleMgr();
            
        }

        private static void RegisterModuleMgr()
        {
            // ModuleMgr     
            ModuleMgrRegister.AutoRegisterModel();
            ModuleMgrRegister.AutoRegisterUIType();
            ModuleMgrRegister.AutoRegisterCtrl();
            ModuleMgrRegister.AutoRegisterUICtrl();

        }


    }
}