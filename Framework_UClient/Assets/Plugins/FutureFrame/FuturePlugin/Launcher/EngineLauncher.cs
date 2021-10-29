using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace FuturePlugin
{
    [DefaultExecutionOrder(-1000)]
    public class EngineLauncher : MonoBehaviour
    {
        public static EngineLauncher Instance { get; private set; }

        public Reporter reporter;
        private Action appMainFunc;

        public void Init(Action _appMainFunc)
        {
            InitAssistSetting();
            LogUtil.Log($"[EngineLauncher]Awake Time:{Time.unscaledTime}");
            Instance = this;
            this.appMainFunc = _appMainFunc;
            Launcher();
        }

        private void Launcher()
        {
            GameObject engineEventSystemGo = new GameObject("[EngineEventSystem]");
            engineEventSystemGo.AddComponent<EngineEventSystem>();
            engineEventSystemGo.transform.SetParent(FutureFrame.Instance.transform, false);
            LogUtil.Log("[EngineLauncher]Launcher EngineEventSystem");
        }

        private void InitAssistSetting()
        {
            LogUtil.EnableLog(LauncherConst.IsEnabledDebugLog);
            
            if (LauncherConst.IsShowUnityLogsViewerReporter)
            {
                string path = "Preset/UnityLogsViewer/Reporter";
                GameObject reporterGo = Instantiate(Resources.Load<GameObject>(path));
                reporterGo.name = "Reporter";
                reporterGo.transform.SetParent(gameObject.transform, false);
                reporterGo.SetActive(LauncherConst.IsEnabledDebugLog);
                reporter = reporterGo.GetComponent<Reporter>();
                reporter.numOfCircleToShow = LauncherConst.ShowUnityLogsViewerReporterCount;
                LogUtil.Log("[EngineLauncher]Init UnityLogsViewer");
            }
        }
    }
}