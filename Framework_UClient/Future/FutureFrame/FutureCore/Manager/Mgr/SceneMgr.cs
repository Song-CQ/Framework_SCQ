/****************************************************
    文件：SceneMgr.cs
	作者：Clear
    日期：2022/1/11 15:2:38
    类型: 框架核心脚本(请勿修改)
	功能：场景管理器
*****************************************************/
using System;
using System.Collections.Generic;

namespace FutureCore
{
    public sealed class SceneMgr:BaseMgr<SceneMgr>
    {
        public const int DefaultMainSceneIdx = 0;

        private Dictionary<int, BaseScene> sceneDict = new Dictionary<int, BaseScene>();

       

        private BaseScene m_currScene;


        public void InitialMain(object param = null)
        {
            if (sceneDict.Count == 0) return;
            LogUtil.LogFormat("[SceneMgr]Start To Init Main Scene {0} Idx", DefaultMainSceneIdx);
            BaseScene scene = GetScene(DefaultMainSceneIdx);
            if (SetScene(scene))
            {
                //momo场景切换
                SceneSwitchMgr.Instance.SwitchInitialScene(DefaultMainSceneIdx, scene.SwitchSceneComplete, param);
            }
        }
        public void SwitchScene(int scenid, object param = null)
        {
            LogUtil.LogFormat("[SceneMgr]Switch Scene To {0} Idx", scenid);
            BaseScene scene = GetScene(scenid);
            if (SetScene(scene))
            {
                //UIMgr.Instance.SwitchSceneCloseAllUI();
                SceneSwitchMgr.Instance.SwitchScene(scenid, scene.SwitchSceneComplete, param);
            }
        }
        public void AddScene(BaseScene baseScene)
        {
            if (!sceneDict.ContainsKey(baseScene.SceneId))
            {
                sceneDict[baseScene.SceneId] = baseScene;
            }
        }
        private bool SetScene(BaseScene scene)
        {
            if (scene == null)
            {
                LogUtil.LogError("[SceneMgr]Set Scene Failed: Scene Is Null");
                return false;
            }
            else if (scene == m_currScene)
            {
                LogUtil.LogError("[SceneMgr]Set Scene Failed: Switch Repetitive Scene");
                return false;
            }
            ///退出当前场景
            if (m_currScene!=null)
            {
                m_currScene.Leave();
            }
            m_currScene = scene;
            m_currScene.Enter();
            return true;
        }

     
        private BaseScene GetScene(int sceneId)
        {
            if (!sceneDict.TryGetValue(sceneId, out BaseScene val))
            {
                LogUtil.LogError("[SceneMgr]No Have This Scene: " + sceneId);
            }
            return val;
        }

    }
}