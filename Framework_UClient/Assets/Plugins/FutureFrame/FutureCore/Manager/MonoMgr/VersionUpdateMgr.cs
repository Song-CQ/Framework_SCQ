/****************************************************
    文件：VersionUpdateMgr.cs
	作者：Clear
    日期：2022/5/4 16:6:35
    类型: 框架核心脚本(请勿修改)
	功能：检测更新资源版本
*****************************************************/
using System;

namespace FutureCore
{
    public class VersionUpdateMgr : BaseMonoMgr<VersionUpdateMgr>
    {
        public void StartUpProcess(Action initAssets)
        {
            //检测资源加载


            initAssets?.Invoke();

        }
    }
}