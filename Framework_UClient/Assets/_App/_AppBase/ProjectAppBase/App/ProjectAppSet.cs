/****************************************************
    文件: ProjectAppSet.cs
    作者: Clear
    日期: 2025/2/28 18:3:4
    类型: 逻辑脚本
    功能: 项目设置
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class ProjectAppSet : MonoBehaviour
    {

        public HotUpdateType HotUpdateType = HotUpdateType.None;


     
        /// <summary>
        /// 是否开启Log
        /// </summary>
        public bool IsEnabledLog = true;
        /// <summary>
        /// 是否要检测资源版本
        /// </summary>
        public bool IsCheckResVer = true;
        /// <summary>
        /// 是否要使用AB包加载资源
        /// </summary>
        public bool IsUseAssetBundlesLoad = false;

        /// <summary>
        /// 使用后是否自动销毁
        /// </summary>
        public bool IsAutoDestroy = true;

        public static void Init()
        {
            ProjectAppSet Set = GameObject.FindObjectOfType<ProjectAppSet>();
            if (Set == null) return;

            AppConst.HotUpdateType = Set.HotUpdateType;
            AppConst.IsEnabledLog = Set.IsEnabledLog;
            AppConst.IsCheckResVer = Set.IsCheckResVer;
            AppConst.IsUseAssetBundlesLoad = Set.IsUseAssetBundlesLoad;



            if (Set.IsAutoDestroy)
            {
                Destroy(Set.gameObject);
            }
            else
            {
                GameObject.DontDestroyOnLoad(Set.gameObject);
            }

        }



    }
}