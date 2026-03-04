/****************************************************
    文件: EditorLauncher.cs
    作者: Clear
    日期: 2026/3/4 17:3:10
    类型: 逻辑脚本
    功能: 编辑器测试场景启动器
*****************************************************/
using System;
using System.Reflection;
using FutureCore;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProjectApp
{

    public class EditorLauncher : MonoBehaviour
    {
 #if UNITY_EDITOR
        public bool Enabled = true;
        private bool IsLauncher = false;
        
        
        public void Awake()
        {
            if (IsLauncher|| !Enabled) return;
            if (SceneManager.GetActiveScene().name == MainLauncher.MainScene) return;

            LogInit.InitLog();
            
            LogUtil.Log("[EditorLauncher]启动".AddColor(ColorType.Green));
            

            Launcher();

            IsLauncher = true;

        }


        private void Launcher()
        {
            //启动项目层
            // Assembly appAssembly = Assembly.GetExecutingAssembly();
            // Type appMainClass = appAssembly.GetType("ProjectApp.Main.AppMain");
            // MethodInfo mainFunc = appMainClass.GetMethod("Main", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            // mainFunc.Invoke(null, null);

            //重新设置AppFacade静态字段
            AppFacadeRedirection.RedirectionStaticField();
            //重新设置App静态字段
            AppRedirection.RedirectionStaticField();




            BaseManagerRegister.Register();
            AppManagerRegister.Register();
            BaseManagerRegister.RegisterData();
            AppManagerRegister.RegisterData();
            AppManagerRegister.RegisterGameLogic();


            App.AppFacadeStartUp();
            GlobalMgr.Instance.StartUp();
  

            

        }

 #endif
    }

}