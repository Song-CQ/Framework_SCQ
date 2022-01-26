/****************************************************
    文件：MainScene.cs
	作者：Clear
    日期：2022/1/13 15:16:43
    类型: 框架核心脚本(请勿修改)
	功能：主场景
*****************************************************/
using FutureCore;

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

                //VersionUpdateMgr.Instance.StartUpProcess(InitAssets);
            }
            else
            {
                //InitAssets();
            }
            UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.MainUI_Open);
        }
        private void InitAssets()
        {
            AppDispatcher.Instance.AddListener(AppMsg.System_AssetsInitComplete, OnAssetsInitComplete);

            LogUtil.Log("[MainScene]Init Assets");
            //App.SetLoadingUI(ProgressState.AssetsInit_20, AppConst.IsLoadingDelay);
            //ResMgr.Instance.InitAssets();
        }

        private void OnAssetsInitComplete(object obj)
        {



        }

        public override void Dispose()
        {

        }
    }
}