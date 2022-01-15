/****************************************************
    文件：UIMgr.cs
	作者：Clear
    日期：2022/1/15 17:52:12
    类型: 框架核心脚本(请勿修改)
	功能：UI管理器
*****************************************************/
using System;

namespace FutureCore
{   

    public class UIMgr : BaseMonoMgr<UIMgr>
    {
        /// <summary>
        /// UI驱动
        /// </summary>
        private BaseUIDriver uiDriver;

        public override void Init()
        {
            base.Init();           
            InItUIMgr();
        }

        private void InItUIMgr()
        {
            LogUtil.Log("[UIMgr]InitUIMgr");
            uiDriver.Init();
        }

        public void RegisterUIDriver(BaseUIDriver _uIDriver)
        {
            uiDriver = _uIDriver;
            uiDriver.Register();
        }
    }
}