/****************************************************
    文件：SceneMgrRegister.cs
	作者：Clear
    日期：2022/1/26 14:57:18
    类型: 自动创建脚本
	功能：Nothing
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public static class SceneMgrRegister 
    {
        public static void AutoRegisterScene()
        {
            SceneMgr sceneMgr = SceneMgr.Instance;
            sceneMgr.AddScene(new MainScene());
        }




    }
}