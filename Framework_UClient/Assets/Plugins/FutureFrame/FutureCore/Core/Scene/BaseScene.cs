/****************************************************
    文件：BaseScene.cs
	作者：Clear
    日期：2022/1/11 15:11:0
    类型: 框架核心脚本(请勿修改)
	功能：基础场景类
*****************************************************/
namespace FutureCore
{
    public abstract class BaseScene 
    {
        public abstract string Name { get; }
        /// <summary>
        /// 场景id
        /// </summary>
        public abstract int SceneId { get; }
        /// <summary>
        /// 资源包预加载id
        /// </summary>
        public abstract int PreLoadId { get; }

        /// <summary>
        /// 进入场景
        /// </summary>
        public void Enter()
        {
            App.DisplayLoadingUI();
            OnEnter();
        }
        /// <summary>
        /// 退出场景
        /// </summary>
        public void Leave()
        {
            OnLeave();
        }
        /// <summary>
        /// 切换场景完成
        /// </summary>
        /// <param name="param"></param>
        public void SwitchSceneComplete(object param = null)
        {
            AppDispatcher.Instance.Dispatch(AppMsg.Scene_Switch, SceneId);
            OnSwitchSceneComplete(param);
        }

        //public void HideLoadingUI(bool isDelay = false)
        //{
        //    App.HideLoadingUI(isDelay);
        //}

        //public abstract AssetLoader GetLoader();

        protected abstract void OnEnter();
        protected abstract void OnLeave();
        protected abstract void OnSwitchSceneComplete(object param);

        public abstract void Dispose();

    }
}