/****************************************************
    文件：UIRegister_FGUI.cs
	作者：Clear
    日期：2022/1/16 19:52:38
    类型: 框架自动创建(请勿修改)
	功能：FGUI包注册
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp
{
    public static class UIRegister_FGUI
    {
        public static void AutoRegisterBinder()
        {
            UI.A000_common.A000_commonBinder.BindAll();
        }


        public static void AutoRegisterCommonPackages(ref List<string> commonPackages)
        {
            commonPackages.Add("A000_common");

        }

    }
}