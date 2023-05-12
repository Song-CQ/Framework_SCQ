using FutureCore;

namespace ProjectApp
{
    public static class AppManagerRegister
    {
        public static void Register()
        {
            GlobalMgr globalMgr = GlobalMgr.Instance;
            //// Mgr
            //globalMgr.AddMgr(CameraMgr.Instance);
            //// MonoMgr
            //globalMgr.AddMgr(AudioMgr.Instance);
            globalMgr.AddMgr(UIMgr.Instance);
        }

        public static void RegisterData()
        {
            //Module
            RegisterModuleMgr();
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

        
        public static void StartUpAfterRegister()
        {
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
        /// ×¢²áUIºËÐÄÇý¶¯
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
        
        private static void RegisterModuleMgr()
        {
            // ModuleMgr     
            if (!AppConst.IsHotUpdateMode)
            {
                ModuleMgrRegister.AutoRegisterModel();
                ModuleMgrRegister.AutoRegisterUIType();
                ModuleMgrRegister.AutoRegisterCtrl();
                ModuleMgrRegister.AutoRegisterUICtrl();
            }
            
        }



    }
}