using FutureCore;
using UnityEngine;
using UnityEngine.Scripting;

namespace ProjectApp.Main
{
    [Preserve]
    public static class AppMain
    {
        [Preserve]
        public static void Main()
        {
            LogUtil.Log($"[AppMain]Main Time: {Time.unscaledTime}".AddColor(ColorType.Green));
            //重新设置AppFacade静态字段
            AppFacadeRedirection.RedirectionStaticField();
            //重新设置App静态字段
            AppRedirection.RedirectionStaticField();
            //初始化项目
            App.InitApplication(ProjectApplication.Instance);
        }

    }
}