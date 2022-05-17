/****************************************************
    文件: ABMgr.cs
    作者: Clear
    日期: 2022/5/17 14:27:16
    类型: 框架核心脚本(请勿修改)
    功能: AssetBundle 管理器
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class AssetBundleMgr : BaseMonoMgr<AssetBundleMgr>
    {

        private Dictionary<string, AssetBundle> loadAssetBundle;

        public override void Init()
        {
            base.Init();
            loadAssetBundle = new Dictionary<string, AssetBundle>();

        }


        public AssetBundle LoadAssetBundle(string _AssetBundleName)
        {
            return null;


        }


    }
}