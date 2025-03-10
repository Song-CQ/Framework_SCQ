﻿/****************************************************
    文件: ModuleMgrRegister.cs
    作者: Clear
    日期: 2024/7/16 15:27:39
    类型: 自动创建
    功能: 模块数据注册
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public static partial class ModuleMgrRegister 
    {
        public static void AutoRegisterModel()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddModel(ModelConst.GameModel,new GameModel());
            moduleMgr.AddModel(ModelConst.MainModel,new MainModel());
            moduleMgr.AddModel(ModelConst.TipsModel,new TipsModel());

        }

        public static void AutoRegisterUIType()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUIType(UIConst.GameUI,typeof(GameUI));
            moduleMgr.AddUIType(UIConst.MainUI,typeof(MainUI));
            moduleMgr.AddUIType(UIConst.TipsUI,typeof(TipsUI));

        }

        public static void AutoRegisterCtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddCtrl(CtrlConst.GameCtrl,new GameCtrl());
            moduleMgr.AddCtrl(CtrlConst.MainCtrl,new MainCtrl());
            moduleMgr.AddCtrl(CtrlConst.TipsCtrl,new TipsCtrl());

        }

        public static void AutoRegisterUICtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUICtrl(UICtrlConst.GameUICtrl,new GameUICtrl());
            moduleMgr.AddUICtrl(UICtrlConst.MainUICtrl,new MainUICtrl());
            moduleMgr.AddUICtrl(UICtrlConst.TipsUICtrl,new TipsUICtrl());

        }

    }
}