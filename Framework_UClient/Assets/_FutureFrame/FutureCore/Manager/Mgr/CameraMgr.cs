/****************************************************
    文件：CameraMgr.cs
	作者：Clear
    日期：2022/1/16 15:16:59
    类型: 框架核心脚本(请勿修改)
	功能：相机管理器
*****************************************************/
using FairyGUI;
using System;
using UnityEngine;

namespace FutureCore
{
    public sealed class CameraMgr :BaseMgr<CameraMgr>
    {

        public Transform mainCameraRoot;
        public GameObject mainCameraGo;
        public Camera mainCamera;

        public Transform uiCameraRoot;
        public GameObject uiCameraGo;
        public Camera uiCamera;

        public override void Init()
        {
            base.Init();
            InitCameraMgr();

            CreateMainCamera();

            CreadUICamera();
        }

        private void InitCameraMgr()
        {
            AppObjConst.CameraGo = new GameObject(AppObjConst.CameraGoName);
            AppObjConst.CameraGo.SetParent(AppObjConst.FutureFrameGo);
        }

        public override void Dispose()
        {
            base.Dispose();
            EngineUtil.Destroy(AppObjConst.CameraGo);
        }


        private void CreateMainCamera()
        {
            if (mainCamera) return;
         
            GameObject mainRoot = new GameObject("[MainCameraRoot]");
            mainRoot.SetParent(AppObjConst.CameraGo);
            mainRoot.transform.position = CameraConst.MainCameraPos;
            mainCameraRoot = mainRoot.transform;

            mainCameraGo = new GameObject("Main_Camera");
            mainCameraGo.SetParent(mainRoot);
            mainCameraGo.transform.localPosition = Vector3.zero;
            mainCameraGo.tag = "MainCamera";
            mainCameraGo.layer = LayerMaskConst.Default;
        
            mainCamera = CreateCamera(mainCameraGo,LayerMaskConst.Everything);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            // 默认不使用后效
            mainCamera.forceIntoRenderTexture = false;

        }

        public void CreadUICamera()
        {
            GameObject root = new GameObject("[UICameraRoot]");
            uiCameraRoot = root.transform;
            root.transform.position = CameraConst.UICameraPos;
            root.SetParent(AppObjConst.CameraGo);

            switch (AppConst.UIDriver)
            {
                case UIDriverEnem.UGUI:
                    CreadUICamera_UGUI();
                    break;
                case UIDriverEnem.FGUI:
                    CreadUICamera_FGUI();
                    break;
                default:
                    CreadUICamera_UGUI();
                    break;
            }
            
           
            
        }
        
        private void CreadUICamera_UGUI()
        {
            if (uiCamera) return;
            uiCamera = CreateCamera(new GameObject("UGUI_Camera"), LayerMaskConst.UI);          
            uiCamera.depth = CameraConst.UICameraDepth;
            uiCamera.orthographic = true;
            uiCamera.orthographicSize = 10;

           

            uiCameraGo = uiCamera.gameObject;
            uiCameraGo.SetParent(uiCameraRoot);
            uiCamera.transform.localPosition = Vector3.zero;

        }
        private void CreadUICamera_FGUI()
        {
            if (uiCamera) return;

            StageCamera.DefaultCameraSize = Screen.height / 2 * AppConst.FGUIRatio;
            StageCamera.DefaultUnitsPerPixel = AppConst.FGUIRatio;

            StageCamera.CheckMainCamera();
            uiCamera = StageCamera.main;
            // 默认不使用后效
            uiCamera.forceIntoRenderTexture = false;          
            uiCameraGo = uiCamera.gameObject;
            uiCameraGo.SetParent(uiCameraRoot);
            uiCamera.transform.localPosition = Vector3.zero;
        }

        public Camera CreateCamera(GameObject cameraGo, int cullingMask)
        {
            Camera cameraCom = cameraGo.AddComponent<Camera>();
            cameraCom.clearFlags = CameraClearFlags.Depth;
            cameraCom.backgroundColor = Color.black;
            if (cullingMask==-1)
            {
                cameraCom.cullingMask = -1;
            }else
            {
                cameraCom.cullingMask = 1 << cullingMask;
            }
            
            cameraCom.nearClipPlane = -30f;
            cameraCom.farClipPlane = 30f;
            cameraCom.rect = new Rect(0, 0, 1f, 1f);
            cameraCom.depth = CameraConst.MainDepth;
            cameraCom.renderingPath = RenderingPath.UsePlayerSettings;
            cameraCom.useOcclusionCulling = false;
            cameraCom.allowHDR = false;
            cameraCom.allowMSAA = false;
            // 默认不使用后效
            cameraCom.forceIntoRenderTexture = false;
            // 启用动态分辨率
            //cameraCom.allowDynamicResolution = true;
            return cameraCom;
        }

       
    }
}