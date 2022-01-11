/****************************************************
    文件：GameMgr.cs
	作者：Clear
    日期：2022/1/11 14:57:44
    类型: 框架核心脚本(请勿修改)
	功能：游戏管理器
*****************************************************/
namespace FutureCore
{
    public sealed class GameMgr : BaseMgr<GameMgr>
    {
        public bool IsPause { get; private set; }

        public void InitialMain()
        {
            LogUtil.Log("[GameMgr]InitialMain");
            SceneMgr.Instance.InitialMain();

        }

        public void EnterMain()
        {
            LogUtil.Log("[GameMgr]EnterMain");
            SceneMgr.Instance.SwitchScene(SceneMgr.DefaultMainSceneIdx);
        }

    }
}