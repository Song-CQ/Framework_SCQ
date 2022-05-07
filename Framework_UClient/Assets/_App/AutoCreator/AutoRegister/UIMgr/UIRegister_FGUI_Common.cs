/****************************************************
    文件: UIRegister_FGUI_Common.cs
    作者: Clear
    日期: 2022/5/3 21:23:14
    类型: 框架自动创建(请勿修改)
    功能: FGUI包注册（通用）
* ****************************************************/
using System.Collections.Generic;
namespace ProjectApp
{
    public static partial class UIRegister_FGUI
    {
        public static void AutoRegisterCommonBinder()
        {
            UI.A001_common.A001_commonBinder.BindAll();
            UI.A002_loading.A002_loadingBinder.BindAll();
        }
        public static void AutoRegisterCommonPackages(ref List<string> commonPackages)
        {
            commonPackages.Add("A001_common");        
        }

    }
}