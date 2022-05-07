using FutureCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public static class MainLaunch
    {

        public static void Init()
        {
            InitModule();





        }


        private static void InitModule()
        {
            ModuleMgrRegister.AutoRegisterModel();
            ModuleMgrRegister.AutoRegisterUIType();
            ModuleMgrRegister.AutoRegisterCtrl();
            ModuleMgrRegister.AutoRegisterUICtrl();
        }


        public static partial class ModuleMgrRegister
        {
            public static void AutoRegisterModel()
            {
                ModuleMgr moduleMgr = ModuleMgr.Instance;
                moduleMgr.AddModel(ModelConst.GameModel, new GameModel());
                moduleMgr.AddModel(ModelConst.MainModel, new MainModel());

            }

            public static void AutoRegisterUIType()
            {
                ModuleMgr moduleMgr = ModuleMgr.Instance;
                moduleMgr.AddUIType(UIConst.GameUI, typeof(GameUI));
                moduleMgr.AddUIType(UIConst.MainUI, typeof(MainUI));

            }

            public static void AutoRegisterCtrl()
            {
                ModuleMgr moduleMgr = ModuleMgr.Instance;
                moduleMgr.AddCtrl(CtrlConst.GameCtrl, new GameCtrl());
                moduleMgr.AddCtrl(CtrlConst.MainCtrl, new MainCtrl());

            }

            public static void AutoRegisterUICtrl()
            {
                ModuleMgr moduleMgr = ModuleMgr.Instance;
                moduleMgr.AddUICtrl(UICtrlConst.GameUICtrl, new GameUICtrl());
                moduleMgr.AddUICtrl(UICtrlConst.MainUICtrl, new MainUICtrl());

            }

        }
    }
}
