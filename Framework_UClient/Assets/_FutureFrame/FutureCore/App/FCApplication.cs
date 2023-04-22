/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

/*
 Author:du
 Time:2017.10.12
*/

using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace FutureCore
{
    public class FCApplication : MonoBehaviour
    {
        public static bool IsAppQuit { get; private set; }
        public bool IsRestart { get; private set; }
        public int RestartCount { get; private set; }
        public bool IsAppFocus { get; private set; }
        public bool IsAppPause { get; private set; }
        public long LastFocusFlaseTime { get; private set; }

        public virtual void Init()
        {
            LogUtil.Log("[Application]Init " + App.GetAppName());
            IsAppFocus = true;
            //Unity3dIL2Cpp.Link();
        }

        public virtual void Enable()
        {
            LogUtil.Log("[Application]Enable " + App.GetAppName());
           
            CreateEnvironment();
        }

        public virtual void Restart()
        {
            LogUtil.Log("[Application]Restart");
            IsRestart = true;
            RestartCount++;
            StartCoroutine(OnRestart());
        }

        public virtual void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        protected virtual void CreateEnvironment()
        {
            AppObjConst.ApplicationGo = gameObject;
            AppObjConst.EngineSingletonGo = new GameObject(AppObjConst.EngineSingletonGoName);
            AppObjConst.EngineSingletonGo.SetParent(AppObjConst.FutureFrameGo);
        }

        private IEnumerator OnRestart()
        {
            DOTween.KillAll();
            ClearAllObjects();
            yield return YieldConst.WaitForEndOfFrame;

            CloseApp();
            EngineUtil.Destroy(AppObjConst.EngineSingletonGo);
            yield return YieldConst.WaitForEndOfFrame;

            Enable();
        }

        private void CloseApp()
        {
            //UIMgr.Instance.DisposeAllUI();
            ModuleMgr.Instance.DisposeAllModule();
            GlobalMgr.Instance.DisposeAllMgr();
            GlobalMgr.Instance.Dispose();
        }

        private void ClearAllObjects()
        {
            GameObject[] otherGos = FindObjectsOfType<GameObject>();
            foreach (GameObject otherGo in otherGos)
            {
                if (IsCanDestroyObj(otherGo))
                {
                    EngineUtil.Destroy(otherGo);
                }
            }
        }

        private bool IsCanDestroyObj(GameObject go)
        {
            if (go.transform.parent == null)
            {
                if (go.name == AppObjConst.LauncherGoName
                    || go.name == AppObjConst.EngineEventSystemGoName
                    || go.name == AppObjConst.ApplicationGoName
                    || go.name == AppObjConst.EngineSingletonGoName
                    || go.name == AppObjConst.DOTweenGoName
                    || go.name == AppObjConst.SuperInvokeGoName)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return IsCanDestroyObj(go.transform.parent.gameObject);
            }
        }

        /// <summary>
        /// 当程序获得焦点时，bool为true；当程序失去焦点时，bool为false；
        /// </summary>
        private void OnApplicationFocus(bool focus)
        {
            LogUtil.Log("[Application]OnApplicationFocus: " + focus);
            IsAppFocus = focus;
            if (focus)
            {
                MainThreadDispatcher.Instance.Dispatch(MainThreadMsg.App_Focus_True, focus);
            }
            else
            {
                LastFocusFlaseTime = (long)TimerMgr.Instance.Timer.GetSeconds();
                AppDispatcher.Instance.Dispatch(AppMsg.App_Focus_False, focus);
            }
        }

        /// <summary>
        /// 当程序转为暂停时，bool为true；当程序转为继续时，bool为false
        /// </summary>
        private void OnApplicationPause(bool pause)
        {
            LogUtil.Log("[Application]OnApplicationPause: " + pause);
            IsAppPause = pause;
            if (pause)
            {
                AppDispatcher.Instance.Dispatch(AppMsg.App_Pause_True, pause);
            }
            else
            {
                MainThreadDispatcher.Instance.Dispatch(MainThreadMsg.App_Pause_False, pause);
            }
        }

        private void OnApplicationQuit()
        {
            LogUtil.Log("[Application]OnApplicationQuit");
            IsAppQuit = true;
            AppDispatcher.Instance.Dispatch(AppMsg.App_Quit);
            CloseApp();
        }

#if UNITY_ANDROID
        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Home))
            {
                OnClickKeyCodeHome();
            }
            if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickKeyCodeEscape();
            }
        }
#endif
        protected virtual void OnClickKeyCodeHome()
        {
            AppDispatcher.Instance.Dispatch(AppMsg.KeyCode_Home);
            LogUtil.Log("[Application]OnClickAppHome");
        }

        protected virtual void OnClickKeyCodeEscape()
        {
            AppDispatcher.Instance.Dispatch(AppMsg.KeyCode_Escape);
            LogUtil.Log("[Application]OnClickAppEscape");
        }
    }
}