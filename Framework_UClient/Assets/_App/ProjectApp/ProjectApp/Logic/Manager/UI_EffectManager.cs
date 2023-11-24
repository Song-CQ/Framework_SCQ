/****************************************************
    文件: UI_EffectManager.cs
    作者: Clear
    日期: 2023/11/23 16:8:25
    类型: 框架核心脚本(请勿修改)
    功能: UI特效管理器
*****************************************************/
using FutureCore;
using System;

namespace ProjectApp
{
    public class UI_EffectManager :BaseMgr<UI_EffectManager>
    {
        public override void Init()
        {
            base.Init();
            
        }

        public override void StartUp()
        {
            base.StartUp();

            AppDispatcher.Instance.AddOnceListener(AppMsg.App_StartUp, OpenUI_Effect);
        }

        private void OpenUI_Effect(object obj)
        {
            UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.FXEffectUI_Open);


        }

        

    }
}