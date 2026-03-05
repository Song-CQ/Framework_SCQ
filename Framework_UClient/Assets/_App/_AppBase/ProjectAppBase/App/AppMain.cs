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
            //��������AppFacade��̬�ֶ�
            AppFacadeRedirection.RedirectionStaticField();
            //��������App��̬�ֶ�
            AppRedirection.RedirectionStaticField();
            //��ʼ����Ŀ
            App.InitApplication(ProjectApplication.Instance);


        }

    }
}