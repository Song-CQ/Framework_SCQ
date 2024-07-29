using FutureCore;
using System;
using System.Reflection;

namespace ProjectApp
{
    /// <summary>
    /// 项目管理器注册
    /// </summary>
    public static class AppManagerRegister
    {
        public static Action GameLogicRegister;
        public static Action GameLogicRegisterData;


        public static void Register()
        {
            GlobalMgr globalMgr = GlobalMgr.Instance;
            //// Mgr
            
            globalMgr.AddMgr(UIMgr.Instance);
            globalMgr.AddMgr(UI_EffectManager.Instance);

        }

        public static void RegisterData()
        {

            // ResMgr
            RegisterPermanentAssets();
            // AssetBundleMgr
            RegisterStaticAssetBundle();
            // UIMgr
            RegisterUIDriver();
            RegisterFont();
            // WSNetMgr
            RegisterProtoLogIgnore();

            

        }

        public static void RegisterGameLogic()
        {
            //注册

            Assembly appAssembly = Assembly.GetExecutingAssembly();
            Type gameLogicRegisterClass = appAssembly.GetType("ProjectApp.GameLogic.GameLogicRegister");
            if (gameLogicRegisterClass==null)
            {
                return;
            }
            MethodInfo Register = gameLogicRegisterClass.GetMethod("Register", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            MethodInfo RegisterData = gameLogicRegisterClass.GetMethod("RegisterData", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            GameLogicRegister = () =>{ Register.Invoke(null,null); } ;
            GameLogicRegisterData = () =>{ RegisterData.Invoke(null,null); } ;
        }



        public static void StartUpAfterRegister()
        {
            TimerMgr.UpData_Event_ToFrame += MainThreadLog.LoopLog;


        }

        private static void RegisterPermanentAssets()
        {
            //Dictionary<string, UAssetType> permanentAssets = new Dictionary<string, UAssetType>();
            //// FairyGUI
            //permanentAssets.Add("Shader/FairyGUI/FairyGUI-BMFont", UAssetType.Shader);
            //permanentAssets.Add("Shader/FairyGUI/FairyGUI-Image", UAssetType.Shader);
            //permanentAssets.Add("Shader/FairyGUI/FairyGUI-Text", UAssetType.Shader);
            //permanentAssets.Add("Shader/FairyGUI/AddOn/FairyGUI-BlurFilter", UAssetType.Shader);
            //// DragonBones
            //permanentAssets.Add("Shader/DragonBones/DB_BlendMode_Grab", UAssetType.Shader);
            //permanentAssets.Add("Shader/DragonBones/DB_BlendMode_UIGrab", UAssetType.Shader);

            //ResMgr resMgr = ResMgr.Instance;
            //resMgr.SetPermanentAssets(permanentAssets);
        }

        private static void RegisterStaticAssetBundle()
        {
            //AssetBundleMgr assetBundleMgr = AssetBundleMgr.Instance;
            //assetBundleMgr.AddStaticAssetBundle("atlas/common.bytes");
            //assetBundleMgr.AddStaticAssetBundle("shared/staticshared.bytes");
            //assetBundleMgr.AddStaticAssetBundle("shared/spinelibshared.bytes");
        }

        /// <summary>
        /// 注册UI核心驱动
        /// </summary>
        public static void RegisterUIDriver()
        {
            UIMgr uiMgr = UIMgr.Instance;
            BaseUIDriver baseUIDriver = null;
            switch (AppConst.UIDriver)
            {
                case UIDriverEnem.UGUI:
                    baseUIDriver = new UGUIDriver();
                    break;
                case UIDriverEnem.FGUI:
                    baseUIDriver = new FGUIDriver();
                    break;
                default:
                    baseUIDriver = new UGUIDriver();
                    break;
            }
            uiMgr.RegisterUIDriver(baseUIDriver);
        }
        private static void RegisterFont()
        {
            UIMgr uiMgr = UIMgr.Instance;
            uiMgr.RegisterDefaultFont("POETSENONE-REGULAR");
            // uiMgr.RegisterFont("Impact");
        }


        private static void RegisterProtoLogIgnore()
        {
            //WSNetMgr wsNetMgr = WSNetMgr.Instance;
            //wsNetMgr.RegisterC2SProtoLogIgnore(WSNetMsg.C2S_heartbeat);
            //wsNetMgr.RegisterS2CProtoLogIgnore(WSNetMsg.S2C_heartbeat);
            //wsNetMgr.RegisterC2SProtoLogIgnore(WSNetMsg.C2S_preferences);
            //wsNetMgr.RegisterC2SProtoLogIgnore(WSNetMsg.S2C_preferences);
        } 
        
        



    }
}