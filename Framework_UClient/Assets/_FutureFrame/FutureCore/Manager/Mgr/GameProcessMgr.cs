/****************************************************
    文件：GameProcessMgr.cs
	作者：Clear
    日期：2022/1/11 14:57:44
    类型: 框架核心脚本(请勿修改)
	功能：游戏进程管理器
*****************************************************/
using System;

namespace FutureCore
{
    public sealed class GameProcessMgr : BaseMgr<GameProcessMgr>
    {
        public bool IsPause { get; private set; }

        public void InitialMain()
        {
            LogUtil.Log("[GameProcessMgr]InitialMain");
            SceneMgr.Instance.InitialMain();

        }

        public void EnterMain()
        {
            LogUtil.Log("[GameProcessMgr]EnterMain");
            SceneMgr.Instance.SwitchScene(SceneMgr.DefaultMainSceneIdx);
        }


    }
}
