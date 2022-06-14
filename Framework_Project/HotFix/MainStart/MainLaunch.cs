using FutureCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp.HotFix
{
    public static class MainLaunch
    {

        public static void Init()
        {
            InitModule();


            LogUtil.Log("热更模块加载完成!");

        }


        private static void InitModule()
        {
            ModuleMgrRegister.AutoRegisterModel();
            ModuleMgrRegister.AutoRegisterUIType();
            ModuleMgrRegister.AutoRegisterCtrl();
            ModuleMgrRegister.AutoRegisterUICtrl();


            ModuleMgr.Instance.InitAllModule();
            ModuleMgr.Instance.StartUpAllModule(); 
        }


      

        
    }
}
