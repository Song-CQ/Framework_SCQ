/****************************************************************************
* ScriptType: 框架 - 基础业务
* 请勿修改!!!
****************************************************************************/

using FutureCore;

namespace ProjectApp
{

    public static class AppFacadeRedirection
    {
        public static void RedirectionStaticField()
        {
            //设置框架属性
            AppFacade_Redirection();
            AppConst_Redirection();
            EncryptConst_Redirection();
        }

        private static void AppFacade_Redirection()
        {
            // Field
            AppFacade_Frame.AppName = AppFacade.AppName;
            AppFacade_Frame.PackageName = AppFacade.PackageName;
            AppFacade_Frame.AESKey = AppFacade.AESKey;
            AppFacade_Frame.AESIVector = AppFacade.AESIVector;
            //AppFacade_Frame.WebSocketUrls = AppFacade.WebSocketUrls;
            //AppFacade_Frame.WebSocketPort = AppFacade.WebSocketPort;
            //AppFacade_Frame.WebSocketTestPort = AppFacade.WebSocketTestPort;
            //AppFacade_Frame.Domain = AppFacade.Domain;
            //AppFacade_Frame.SDKApiPrefix = AppFacade.SDKApiPrefix;
            //AppFacade_Frame.BuglyAppIDForAndroid = AppFacade.BuglyAppIDForAndroid;
            //AppFacade_Frame.BuglyAppIDForiOS = AppFacade.BuglyAppIDForiOS;
            //AppFacade_Frame.IsWeakNetwork = AppFacade.IsWeakNetwork;
            // AppFacade_Frame.CustomSDK = AppFacade.CustomSDK;
            AppFacade_Frame.IsUseUGameAndroid = AppFacade.IsUseUGameAndroid;
            //AppFacade_Frame.IsOfflineGame = AppFacade.IsOfflineGame;
            AppFacade_Frame.AppDesc = AppFacade.AppDesc;
            AppFacade_Frame.ServerAssestUrl = AppFacade.ServerAssestUrl;
            // Func
            AppFacade_Frame.InitFunc = AppFacade.InitFunc;
            AppFacade_Frame.StartUpFunc = AppFacade.StartUpFunc;
            AppFacade_Frame.GameStartFunc = AppFacade.GameStartFunc;
        }

        private static void AppConst_Redirection()
        {
            AppConst.PackageName = AppFacade_Frame.PackageName;
        }

        private static void EncryptConst_Redirection()
        {
            EncryptConst.AES_Key = AppFacade_Frame.AESKey;
            EncryptConst.AES_IVector = AppFacade_Frame.AESIVector;
        }
    }
}