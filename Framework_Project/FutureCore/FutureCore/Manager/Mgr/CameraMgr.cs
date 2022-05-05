/****************************************************
    文件：CameraMgr.cs
	作者：Clear
    日期：2022/1/16 15:16:59
    类型: 框架核心脚本(请勿修改)
	功能：相机管理器
*****************************************************/
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
            string name = "MainCamera";
           
            GameObject mainRoot = new GameObject(name + "Root");
            mainRoot.SetParent(AppObjConst.CameraGo);
            mainRoot.transform.position = CameraConst.MainCameraPos;
            mainCameraRoot = mainRoot.transform;

            mainCameraGo = new GameObject(name);
            mainCameraGo.SetParent(mainRoot);
            mainCameraGo.transform.localPosition = Vector3.zero;
            mainCameraGo.tag = name;
            mainCameraGo.layer = LayerMaskConst.Default;
                       
            mainCamera = CreateCamera(mainCameraGo,LayerMaskConst.Default);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            // 默认不使用后效
            mainCamera.forceIntoRenderTexture = false;

        }

        public void CreadUICamera()
        {
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
            
        }
        private void CreadUICamera_FGUI()
        {

            if (uiCamera) return;

            uiCamera = UIMgr.Instance.GetUICamera();
            uiCamera.depth = CameraConst.UICameraDepth;
            // 默认不使用后效
            uiCamera.forceIntoRenderTexture = false;
            
            uiCameraGo = uiCamera.gameObject;

            GameObject root = new GameObject("FGUICameraRoot");
            uiCameraRoot = root.transform;
            root.transform.position = CameraConst.UICameraPos;
            root.SetParent(AppObjConst.CameraGo);
            uiCameraGo.SetParent(root);
          

           
        }

        public Camera CreateCamera(GameObject cameraGo, int cullingMask)
        {
            Camera cameraCom = cameraGo.AddComponent<Camera>();
            cameraCom.clearFlags = CameraClearFlags.Depth;
            cameraCom.backgroundColor = Color.black;
            cameraCom.cullingMask = cullingMask;
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