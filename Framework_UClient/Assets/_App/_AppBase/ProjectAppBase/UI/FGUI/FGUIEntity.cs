/****************************************************
    文件：FGUIEntity.cs
	作者：Clear
    日期：2022/1/25 16:28:51
    类型: 框架核心脚本(请勿修改)
	功能：FGUI实体
*****************************************************/
using FairyGUI;
using FutureCore;
using System;

namespace ProjectApp
{
    public class FGUIEntity:UIEntity
    {
        public GComponent ui;
        public GGraph UIMask;

        public GTweener openUiGTweener;
        public GTweener closeUiGTweener;

        public FGUIEntity(GComponent _ui)
        {
            ui = _ui;
        }

        public override void SetVisible(bool arse)
        {
            if (ui == null) return;
            ui.visible = arse;
        }
        protected override void SetName(string uiName)
        {
            if (ui == null) return;
            ui.gameObjectName = uiName;       
        }

        public override void OpenUIAnim(Action onComplete)
        {
            ui.pivot = VectorConst.Half;
            ui.SetScale(UIMgrConst.OpenUIAnimEffectScale.x, UIMgrConst.OpenUIAnimEffectScale.y);
            ui.touchable = true;
            openUiGTweener = ui.TweenScale(VectorConst.One, UIMgrConst.UIAnimEffectTime).SetIgnoreEngineTimeScale(true).SetEase(EaseType.BackInOut).OnComplete(() => {
                openUiGTweener = null;
                onComplete?.Invoke(); 
            });
        }

        public override void CloseUIAnim(Action onComplete)
        {
            ui.touchable = false;
            closeUiGTweener = ui.TweenScale(UIMgrConst.OpenUIAnimEffectScale, UIMgrConst.UIAnimEffectTime).SetIgnoreEngineTimeScale(true).SetEase(EaseType.BackIn).OnComplete(() => { 
            
                closeUiGTweener = null;
                onComplete?.Invoke();
            });
        }

        public override void Dispose()
        {
            ui.Dispose();
            ui = null;
        }
    }
}