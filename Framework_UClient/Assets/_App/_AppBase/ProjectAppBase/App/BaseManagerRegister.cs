using FutureCore;

namespace ProjectApp
{
    /// <summary>
    /// 基础管理器注册
    /// </summary>
    public static class BaseManagerRegister
    {
        public static void Register()
        {
            GlobalMgr globalMgr = GlobalMgr.Instance;
            // 模块管理器
            globalMgr.AddMgr(ModuleMgr.Instance);

            //// PreMonoMgr
            //globalMgr.AddMgr(AssistDebugMgr.Instance);

            //// Mgr
           
            globalMgr.AddMgr(CameraMgr.Instance);
            //globalMgr.AddMgr(ConfigMgr.Instance);
            globalMgr.AddMgr(ConfigDataMgr.Instance);
            //globalMgr.AddMgr(ConsoleMgr.Instance);
            //globalMgr.AddMgr(DateTimeMgr.Instance);
            globalMgr.AddMgr(TimerMgr.Instance);
            //globalMgr.AddMgr(DispatcherMgr.Instance);
            globalMgr.AddMgr(FutureCore.GameMgr.Instance);
            //globalMgr.AddMgr(GMMgr.Instance);
            //globalMgr.AddMgr(GraphicsMgr.Instance);
            //globalMgr.AddMgr(LoadPipelineMgr.Instance);
            //globalMgr.AddMgr(ResMgr.Instance);
            globalMgr.AddMgr(SceneMgr.Instance);
            //globalMgr.AddMgr(VersionMgr.Instance);
            //globalMgr.AddMgr(WSNetMgr.Instance);
            //globalMgr.AddMgr(WSNetProxyMgr.Instance);

            //// MonoMgr
            ////globalMgr.AddMgr(AudioMgr.Instance);
            globalMgr.AddMgr(InputMgr.Instance);
            globalMgr.AddMgr(AssetBundleMgr.Instance);
            globalMgr.AddMgr(CoroutineMgr.Instance);
            globalMgr.AddMgr(HttpMgr.Instance);
            globalMgr.AddMgr(DownloadTaskMgr.Instance);
            //globalMgr.AddMgr(ScreenshotMgr.Instance);
            //globalMgr.AddMgr(TestMgr.Instance);
            ////globalMgr.AddMgr(ThreadMgr.Instance);
            //globalMgr.AddMgr(TickMgr.Instance);
            globalMgr.AddMgr(VersionUpdateMgr.Instance);
            globalMgr.AddMgr(ILRuntimeMgr.Instance);
            //globalMgr.AddMgr(WorldSpaceMgr.Instance);

            //// AppMgr
            //globalMgr.AddMgr(ChannelMgr.Instance);
            //globalMgr.AddMgr(GameDataMgr.Instance);
            //globalMgr.AddMgr(PreferencesMgr.Instance);


            //// AppMonoMgr
            //globalMgr.AddMgr(AntiCheatMgr.Instance);
            //globalMgr.AddMgr(TestMgr_Logic.Instance);
        }

        public static void RegisterData()
        {
            // SceneMgr
            SceneMgrRegister.AutoRegisterScene();
            //// ConfigMgr
            //ConfigMgrRegister.AutoRegisterConfig();
            //// WSNetMgr
            //WSNetMgrRegister.AutoRegisterProtoType();
            
            ModuleMgrRegister.RegisterCommom();
            //// UIMgr
            //UIMgrRegister.AutoRegisterBinder();
            //UIMgrRegister.AutoRegisterCommonPackages();
        }


    }

}

