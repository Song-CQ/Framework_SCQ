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

        public override int SceneId => SceneConst.MainIdx;

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
                App.SetLoadingSchedule(ProgressState.VersionUpdate);
                VersionUpdateMgr.Instance.StartUpProcess(InitAssets);
            }
            else
            {
                InitAssets();
            }
        }

        private void InitAssets()
        {
            AppDispatcher.Instance.AddOnceListener(AppMsg.System_AssetsInitComplete, OnAssetsInitComplete);

            LogUtil.Log("[MainScene]Init Assets");            
            App.SetLoadingSchedule(ProgressState.AssetsInit);
            
            ResMgr.Instance.InitAssets();
           

        }

        private void OnAssetsInitComplete(object obj)
        {
            AppDispatcher.Instance.AddOnceListener(AppMsg.System_LoadHotFixComplete, OnLoadHotFixComplete);
            LogUtil.Log("[MainScene]Load HotFix");
            App.SetLoadingSchedule(ProgressState.LoadHotFix);
            //加载热更代码
            ILRuntimeMgr.Instance.StartLoadHotFix();
            

        }

        private void OnLoadHotFixComplete(object obj)
        {
            
            App.SetLoadingSchedule(ProgressState.ConfigInit);
            AppDispatcher.Instance.AddOnceListener(AppMsg.System_ConfigInitComplete, LoadComplete);

            //to do
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
            
          
            CtrlDispatcher.Instance.Dispatch(CtrlMsg.Game_StartReady);
            
            TimerUtil.Simple.AddTimer(AppConst.GameStartReadyDelayTime, () => {

                ModuleMgr.Instance.AllModuleGameStart();
                App.HideLoadingUI();

                CtrlDispatcher.Instance.Dispatch(CtrlMsg.Game_StartBefore);
                CtrlDispatcher.Instance.Dispatch(CtrlMsg.Game_Start);
                CtrlDispatcher.Instance.Dispatch(CtrlMsg.Game_StartLater);

            });
            

        }

        public override void Dispose()
        {

        }
    }
}