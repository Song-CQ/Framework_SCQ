/****************************************************
    文件：FGUIDriver.cs
	作者：Clear
    日期：2022/1/15 18:9:24
    类型: 框架基础脚本(请勿修改)
	功能：Fgui UI驱动
*****************************************************/
using FairyGUI;
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class FGUIDriver : BaseUIDriver
    {
        private Vector2 uiCenterPos;
        private Dictionary<int, Window> uiLayerWindowDict = new Dictionary<int, Window>();
        private List<string> commonPackageList = new List<string>();
        public override void Register()
        {
            UIRegister_FGUI.AutoRegisterBinder();
            UIRegister_FGUI.AutoRegisterCommonPackages(ref commonPackageList);

        }

        public override void Init()
        {
            InitFguiConfig();
            InitFguiSettings();
            InitFguiCommonPackages();
        }

        /// <summary>
        /// Fgui基础设置
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InitFguiConfig()
        {


        }

        private void InitFguiSettings()
        {
            AppObjConst.UICacheGo = new GameObject(AppObjConst.UICacheGoName);
            AppObjConst.UICacheGo.SetParent(AppObjConst.FutureFrameGo);

            //DisplayObject.CreateUICacheRoot(AppObjConst.UICacheGo.transform);

            Stage.Instantiate();
            Stage.inst.gameObject.transform.SetParent(AppObjConst.UIGo.transform);
            ///设置ui的分辨率
            Vector2Int uiResolution = AppConst.UIResolution;
            GRoot.inst.SetContentScaleFactor(uiResolution.x, uiResolution.y,
                UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            uiCenterPos = new Vector2(GRoot.inst.width / 2f, GRoot.inst.height / 2f);

        }

        private void InitFguiCommonPackages()
        {
            UIPackage.RemoveAllPackages();
            foreach (var PackageName in commonPackageList)
            {
                ResMgr.Instance.AddFguiPackage(PackageName, GetPackageUIPath(PackageName));

            }

        }
        private string GetPackageUIPath(string packageName)
        {
            return string.Format("FGUI/{0}", packageName);
        }



        public override Transform CreadUILayer(int index, string name)
        {
            Window uiLayerWindow = new Window();
            uiLayerWindow.fairyBatching = false;
            uiLayerWindow.name = name;
            uiLayerWindow.displayObject.name = name;
            uiLayerWindow.gameObjectName = uiLayerWindow.name;
            uiLayerWindow.sortingOrder = index * 100;
            uiLayerWindow.Show();
            uiLayerWindow.fairyBatching = false;
            return uiLayerWindow.displayObject.cachedTransform;
        }
        public override void Dispose()
        {

        }


    }
}