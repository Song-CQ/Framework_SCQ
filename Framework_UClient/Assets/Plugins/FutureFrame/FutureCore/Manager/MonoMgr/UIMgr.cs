/****************************************************
    文件：UIMgr.cs
	作者：Clear
    日期：2022/1/15 17:52:12
    类型: 框架核心脚本(请勿修改)
	功能：UI管理器
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{   

    public class UIMgr : BaseMonoMgr<UIMgr>
    {
        /// <summary>
        /// UI驱动
        /// </summary>
        private BaseUIDriver uiDriver;

        private Dictionary<int,Transform> uiLayerWindowDict;


        public void RegisterUIDriver(BaseUIDriver _uIDriver)
        {
            uiDriver = _uIDriver;
            uiDriver.Register();
        }

        public override void Init()
        {
            base.Init();           
            InItUIMgr();
        }

        private void InItUIMgr()
        {
            LogUtil.Log("[UIMgr]InitUIMgr");
            GameObject uiRootParent = new GameObject(AppObjConst.UIGoName);
            AppObjConst.UIGo = uiRootParent;
            AppObjConst.UIGo.layer = LayerMaskConst.UI;
            AppObjConst.UIGo.SetParent(AppObjConst.FutureFrameGo);
            ///驱动初始化
            uiDriver.Init();

            InitLayers();
          
        }

     

        /// <summary>
        /// 初始化UI层级
        /// </summary>
        private void InitLayers()
        {
            uiLayerWindowDict = new Dictionary<int, Transform>();
            for (int i = 0; i < UILayerConst.AllUILayer.Length; i++)
            {
                string name = UILayerConst.AllUILayer[i];
                Transform uiLayerWindow =  uiDriver.CreadUILayer(i,name);
                  
                uiLayerWindowDict.Add(i,uiLayerWindow);
            }
        }
        

        public override void Dispose()
        {
            base.Dispose();
            uiDriver.Dispose();
        }

    }
}