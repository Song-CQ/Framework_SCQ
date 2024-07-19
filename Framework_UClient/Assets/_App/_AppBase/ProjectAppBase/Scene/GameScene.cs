/****************************************************
    文件：GameScene.cs
	作者：Clear
    日期：2022/1/13 15:16:43
    类型: 框架核心脚本(请勿修改)
	功能：主场景
*****************************************************/
using FutureCore;
using System;

namespace ProjectApp
{
    public class GameScene : BaseScene
    {
        public override string Name => SceneConst.GameName;

        public override int SceneId => SceneConst.GameIndex;

        public override int PreLoadId => PreLoadIdConst.GameScene;



        protected override void OnEnter()
        {
            LogUtil.Log("[GameScene]Enter");

         
        }

        protected override void OnLeave()
        {
            LogUtil.Log("[GameScene]Leave");

        }

        protected override void OnSwitchSceneComplete(object param)
        {
            LogUtil.Log("[GameScene]SwitchSceneComplete");

            ShowScene();
        }        

       

       

        private void ShowScene()
        {
            LogUtil.Log("[GameScene]Show Scene");
            
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