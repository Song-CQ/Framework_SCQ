using FutureCore;
using ProjectApp.GameLogic;
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
            Register();//注册


            LogUtil.Log("热更模块加载完成!");
            LogUtil.Log("真加载完成!");

        }

       

        public static void Register()
        {

            AppManagerRegister.GameLogicRegister = GameLogicRegister.Register;
            AppManagerRegister.GameLogicRegisterData = GameLogicRegister.RegisterData;

            //Module
            RegisterModuleMgr();

         
        }  
       


        private static void RegisterModuleMgr()
        {
            ModuleMgrRegister.AutoRegisterModel();
            ModuleMgrRegister.AutoRegisterUIType();
            ModuleMgrRegister.AutoRegisterCtrl();
            ModuleMgrRegister.AutoRegisterUICtrl();


            
        }


      

        
    }
}
