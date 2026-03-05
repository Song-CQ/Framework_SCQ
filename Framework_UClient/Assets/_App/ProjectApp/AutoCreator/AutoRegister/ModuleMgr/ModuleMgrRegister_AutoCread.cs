/****************************************************
    文件: ModuleMgrRegister.cs
    作者: Clear
    日期: 2026/3/5 18:7:5
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
            moduleMgr.AddModel(ModelConst.GameWinModel,new GameWinModel());
            moduleMgr.AddModel(ModelConst.MainModel,new MainModel());
            moduleMgr.AddModel(ModelConst.TipsModel,new TipsModel());
            moduleMgr.AddModel(ModelConst.UsePropModel,new UsePropModel());

        }

        public static void AutoRegisterUIType()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUIType(UIConst.GameUI,typeof(GameUI));
            moduleMgr.AddUIType(UIConst.GameWinUI,typeof(GameWinUI));
            moduleMgr.AddUIType(UIConst.MainUI,typeof(MainUI));
            moduleMgr.AddUIType(UIConst.TipsUI,typeof(TipsUI));
            moduleMgr.AddUIType(UIConst.UsePropUI,typeof(UsePropUI));

        }

        public static void AutoRegisterCtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddCtrl(CtrlConst.GameCtrl,new GameCtrl());
            moduleMgr.AddCtrl(CtrlConst.GameWinCtrl,new GameWinCtrl());
            moduleMgr.AddCtrl(CtrlConst.MainCtrl,new MainCtrl());
            moduleMgr.AddCtrl(CtrlConst.TipsCtrl,new TipsCtrl());
            moduleMgr.AddCtrl(CtrlConst.UsePropCtrl,new UsePropCtrl());

        }

        public static void AutoRegisterUICtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
            moduleMgr.AddUICtrl(UICtrlConst.GameUICtrl,new GameUICtrl());
            moduleMgr.AddUICtrl(UICtrlConst.GameWinUICtrl,new GameWinUICtrl());
            moduleMgr.AddUICtrl(UICtrlConst.MainUICtrl,new MainUICtrl());
            moduleMgr.AddUICtrl(UICtrlConst.TipsUICtrl,new TipsUICtrl());
            moduleMgr.AddUICtrl(UICtrlConst.UsePropUICtrl,new UsePropUICtrl());

        }

    }
}