/****************************************************************************
* ScriptType: 框架 - 基础业务
* 请勿修改!!!
****************************************************************************/

using FutureCore;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace ProjectApp
{
    public class ProjectApplication : FCApplication
    {
        private static ProjectApplication m_instance;
        public static ProjectApplication Instance
        {
            get
            {
                if (m_instance == null)
                {
                    if (IsAppQuit)
                    {
                        return null;
                    }
                    AppObjConst.ApplicationGo = new GameObject(AppObjConst.ApplicationGoName);
                    AppObjConst.ApplicationGo.SetParent(AppObjConst.FutureFrameGo);
                    m_instance = AppObjConst.ApplicationGo.AddComponent<ProjectApplication>();
                }
                return m_instance;
            }
        }

        [HideInInspector]
        public bool IsNewInstall { get; private set; }

        public override void Init()
        {
            base.Init();
            if (!PrefsUtil.HasKey(PrefsKeyConst.App_isNewInstall))
            {
                IsNewInstall = true;
                PrefsUtil.WriteInt(PrefsKeyConst.App_isNewInstall, 1);
            }
            else
            {
                IsNewInstall = false;
            }
        }

        public override void Enable()
        {
            App.AppFacadeInit();
            base.Enable();

           // SDKGlobal.Init();
            InitPlugin();
            InitDefine();
            InitPreSetting();
            InitAppSetting();

       

            BaseManagerRegister.Register();
            //AppManagerRegister.Register();
            //BaseManagerRegister.RegisterData();
            //AppManagerRegister.RegisterData();

            App.AppFacadeStartUp();
            //GlobalMgr.Instance.StartUp();
            //AppManagerRegister.StartUpAfterRegister();

            AfterInitDefine();
            InitSettingMode();

            StartUpGameMain();
        }

        private void OnDestroy()
        {
            m_instance = null;
        }

        #region Enable

        /// <summary>
        /// 初始化第三方Plugin
        /// </summary>
        private void InitPlugin()
        {
            DOTweenHelper.Init();
     
        }

        private void InitDefine()
        {
            AppConst.Init();
           // ABPakConst.Init();
            ColorConst.Init();
        }
        /// <summary>
        /// 项目基础设置
        /// </summary>
        private void InitPreSetting()
        {           
            // PC测试模式分辨率
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Screen.SetResolution(AppConst.PCTestResolution.x, AppConst.PCTestResolution.y, false);
#endif
        }
       
        /// <summary>
        /// 初始化项目设置
        /// </summary>
        private void InitAppSetting()
        {
            if (!AppConst.UseInternalSetting) return;
            /// 引擎设置
            // 可选择禁用物理模拟 手动调用Physics.Simulate/Physics2D.Simulate来模拟
            Physics.autoSimulation = true;
            Physics.autoSyncTransforms = true;
            Physics2D.autoSimulation = true;
            Physics2D.autoSyncTransforms = true;
            // 项目设置
            Debug.unityLogger.logEnabled = AppConst.IsEnabledEngineLog;
            Debug.unityLogger.filterLogType = AppConst.EnabledFilterLogType;
            Screen.sleepTimeout = AppConst.SleepTimeoutMode;
            Application.runInBackground = AppConst.IsRunInBG;
            QualitySettings.vSyncCount = 0;
            QualitySettings.lodBias = 1;
            QualitySettings.antiAliasing = AppConst.AntiAliasing;
            Application.targetFrameRate = AppConst.LowFrameRate;
            AppConst.UpdateFrameRate();
            // Profiler
            InitProfilerSetting();
        }
        
        /// <summary>
        /// 初始化性能分析
        /// </summary>
        private void InitProfilerSetting()
        {
            if (AppConst.IsDevelopmentBuild)
            {
                Profiler.enabled = AppConst.IsEnabledEngineProfiler;
                Profiler.SetAreaEnabled(ProfilerArea.CPU, true);
                Profiler.SetAreaEnabled(ProfilerArea.GPU, true);
                Profiler.SetAreaEnabled(ProfilerArea.Rendering, true);
                Profiler.SetAreaEnabled(ProfilerArea.Memory, true);
                Profiler.SetAreaEnabled(ProfilerArea.Audio, true);
                Profiler.SetAreaEnabled(ProfilerArea.Video, false);
                Profiler.SetAreaEnabled(ProfilerArea.Physics, false);
                Profiler.SetAreaEnabled(ProfilerArea.Physics2D, false);
                Profiler.SetAreaEnabled(ProfilerArea.NetworkMessages, false);
                Profiler.SetAreaEnabled(ProfilerArea.NetworkOperations, false);
                Profiler.SetAreaEnabled(ProfilerArea.UI, false);
                Profiler.SetAreaEnabled(ProfilerArea.UIDetails, false);
                Profiler.SetAreaEnabled(ProfilerArea.GlobalIllumination, false);
            }
        }

        private void AfterInitDefine()
        {
            //AppConst.AfterInit();
            //NetConst.AfterInit();
        }

        private void StartUpGameMain()
        {
            LogUtil.Log("[Application]StartUpGameMain");
            //if (!IsRestart)
            //{
            //    GameMgr.Instance.InitialMain();
            //}
            //else
            //{
            //    GameMgr.Instance.EnterMain();
            //}
        }

        #endregion Enable

        #region SettingMode

        private void InitSettingMode()
        {
            if (!AppConst.UseInternalSetting) return;
            InitResolutionMode();
            InitFrameRateMode();
        }

        private bool isHDMode;
        public bool IsHDMode
        {
            get
            {
                return isHDMode;
            }
            set
            {
                isHDMode = value;
                //PrefsUtil.WriteBool(FutureCore.PrefsKeyConst.Application_isHDMode, isHDMode);
                SetResolutionMode(isHDMode);
            }
        }

        private bool isHFRMode;
        public bool IsHFRMode
        {
            get
            {
                return IsHFRMode;
            }
            set
            {
                isHFRMode = value;
               // PrefsUtil.WriteBool(FutureCore.PrefsKeyConst.Application_isHFRMode, isHFRMode);
                SetFrameRateMode(isHFRMode);
            }
        }

        private void InitResolutionMode()
        {
            //isHDMode = PrefsUtil.ReadBool(FutureCore.PrefsKeyConst.Application_isHDMode, true);
            SetResolutionMode(isHDMode);
        }

        private void InitFrameRateMode()
        {
            //isHFRMode = PrefsUtil.ReadBool(FutureCore.PrefsKeyConst.Application_isHFRMode, true);
            SetFrameRateMode(isHFRMode);
        }

        private void SetResolutionMode(bool isHDMode)
        {
            if (isHDMode)
            {
                ScreenConst.CurrResolution.x = ScreenConst.RawResolution.x * AppConst.HDHighViewScale;
                ScreenConst.CurrResolution.y = ScreenConst.RawResolution.y * AppConst.HDHighViewScale;
            }
            else
            {
                ScreenConst.CurrResolution.x = ScreenConst.RawResolution.x * AppConst.HDLowViewScale;
                ScreenConst.CurrResolution.y = ScreenConst.RawResolution.y * AppConst.HDLowViewScale;
            }
            SetScreenResolution(ScreenConst.CurrResolution.x, ScreenConst.CurrResolution.y, true);
        }

        private void SetFrameRateMode(bool isHFRMode)
        {
            if (isHFRMode)
            {
                Application.targetFrameRate = AppConst.HighFrameRate;
            }
            else
            {
                Application.targetFrameRate = AppConst.LowFrameRate;
            }

            // 限制帧率
            QualitySettings.vSyncCount = 0;
            QualitySettings.lodBias = 1;
            AppConst.UpdateFrameRate();
        }

        private void SetScreenResolution(float width, float height, bool isFullScreen)
        {
            StartCoroutine(OnSetScreenResolution(width, height, isFullScreen));
        }

        private IEnumerator OnSetScreenResolution(float width, float height, bool isFullScreen)
        {
            yield return YieldConst.WaitForEndOfFrame;
            Camera[] allCams = Camera.allCameras;
            if (allCams == null)
            {
                yield break;
            }
            foreach (Camera cam in allCams)
            {
                cam.enabled = false;
            }
            Screen.SetResolution((int)width, (int)height, isFullScreen);
            Screen.fullScreen = true;

            yield return YieldConst.WaitForEndOfFrame;
            if (allCams == null)
            {
                yield break;
            }
            foreach (Camera cam in allCams)
            {
                cam.enabled = true;
            }
        }

        #endregion SettingMode
    }
}