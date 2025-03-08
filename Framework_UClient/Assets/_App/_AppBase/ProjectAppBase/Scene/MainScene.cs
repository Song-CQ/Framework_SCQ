/****************************************************
    文件：MainScene.cs
	作者：Clear
    日期：2022/1/13 15:16:43
    类型: 框架核心脚本(请勿修改)
	功能：主场景
*****************************************************/
using FutureCore;
using System;

namespace ProjectApp
{
    public class MainScene : BaseScene
    {
        public override string Name => SceneConst.MainName;

        public override int SceneId => SceneConst.MainIndex;

        public override int PreLoadId => PreLoadIdConst.MainScene;



        protected override void OnEnter()
        {
            LogUtil.Log("[MainScene]Enter");

            AppGlobal.IsGameStart = false;
        }

        protected override void OnLeave()
        {
            LogUtil.Log("[MainScene]Leave");
            // LoadPipelineMgr.Instance.UnloadPreLoad(PreLoadId);
        }

        protected override void OnSwitchSceneComplete(object param)
        {
            LogUtil.Log("[MainScene]SwitchSceneComplete");

            //统计

            StartUpAppProcess();
        }

        private void StartUpAppProcess()
        {
            LogUtil.Log("[MainScene]Start Up App Process");
            AppDispatcher.Instance.Dispatch(AppMsg.App_StartUp);

            
            // 初始化资源
            if (!AppConst.IsDevelopMode)
            {
                //检测版本资源更新
                App.SetLoadingSchedule(ProgressState.AssetsPrepare);
                VersionUpdateMgr.Instance.StartUpProcess(InitAssets);
            }
            else
            {
                InitAssets(true);
            }
        }

        private void InitAssets(bool isComplete)
        {
            AppDispatcher.Instance.AddOnceListener(AppMsg.System_AssetsInitComplete, OnAssetsInitComplete);

            LogUtil.Log("[MainScene]Init Assets");            
            App.SetLoadingSchedule(ProgressState.AssetsInit);
            
            ResMgr.Instance.InitAssets();
           

        }

        private void OnAssetsInitComplete(object obj)
        {
            if (AppConst.HotUpdateType == HotUpdateType.None)
            {
                OnLoadHotFixComplete(null);
            }
            else if (AppConst.HotUpdateType == HotUpdateType.ILRuntime)
            {
                AppDispatcher.Instance.AddOnceListener(AppMsg.System_LoadHotFixComplete, OnLoadHotFixComplete);
                LogUtil.Log("[MainScene]Load HotFix");
                App.SetLoadingSchedule(ProgressState.LoadHotFix);


                //加载热更代码
                ILRuntimeMgr_Register.RegisterAll(ILRuntimeMgr.Instance.AppDomain);
                ILRuntimeMgr.Instance.StartLoadHotFix();
            }
            else
            {
                LogUtil.LogError("代码热重载错误！请检查： HotUpdateType");
            }
            
            

        }

        private void OnLoadHotFixComplete(object obj)
        {

            AppManagerRegister.GameLogicRegister?.Invoke();
            AppManagerRegister.GameLogicRegisterData?.Invoke();

            LogUtil.Log("[MainScene]Check Mgr StartUp");
            GlobalMgr.Instance.CheckStartUp();
            LogUtil.Log("[MainScene]InitAllModule");
            ModuleMgr.Instance.InitAllModule();
            LogUtil.Log("[MainScene]StartUpAllModule");
            ModuleMgr.Instance.StartUpAllModule();

            App.SetLoadingSchedule(ProgressState.ConfigInit);
            AppDispatcher.Instance.AddOnceListener(AppMsg.System_ConfigInitComplete, LoadComplete);

            //to do 读取数据

            LoadConfigInit();

        }

        /// <summary>
        /// 加载Config资源
        /// </summary>
        private void LoadConfigInit()
        {

            //先重置 再设置
            ConfigDataMgr.Instance.ResetData();

            ConfigDataMgr.Instance.ReadData();
            LogUtil.Log("[MainScene]AllModuleReadData");
            ModuleMgr.Instance.AllModuleReadData();
            AppDispatcher.Instance.Dispatch(AppMsg.System_ConfigInitComplete);
        }
        

        private void LoadComplete(object o)
        {
            App.SetLoadingSchedule(ProgressState.ShowScene);
            TimerUtil.Simple.AddTimer(AppConst.LoadingCompleteDelayTime, () => ShowScene());
       
        }

        private void ShowScene()
        {
            LogUtil.Log("[MainScene]Show Scene");
            

            SceneMgr.Instance.SwitchScene(SceneConst.GameIndex);
        }

        public override void Dispose()
        {

        }
    }
}