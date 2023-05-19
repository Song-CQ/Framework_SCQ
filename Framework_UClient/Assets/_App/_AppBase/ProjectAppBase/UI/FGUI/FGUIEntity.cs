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
        public GComponent UI { get; private set; }
        public GGraph UIMask;

        public GTweener openUiGTweener;
        public GTweener closeUiGTweener;

        public FGUIEntity(GComponent _ui)
        {
            UI = _ui;
        }

        public override void SetVisible(bool arse)
        {
            if (UI == null) return;
            UI.visible = arse;
        }
        protected override void SetName(string uiName)
        {
            if (UI == null) return;
            UI.gameObjectName = uiName;       
        }

        public override void OpenUIAnim(Action onComplete)
        {
            UI.pivot = VectorConst.Half;
            UI.SetScale(UIMgrConst.OpenUIAnimEffectScale.x, UIMgrConst.OpenUIAnimEffectScale.y);
            UI.touchable = true;
            openUiGTweener = UI.TweenScale(VectorConst.One, UIMgrConst.UIAnimEffectTime).SetIgnoreEngineTimeScale(true).SetEase(EaseType.BackInOut).OnComplete(() => {
                openUiGTweener = null;
                onComplete?.Invoke(); 
            });
        }

        public override void CloseUIAnim(Action onComplete)
        {
            UI.touchable = false;
            closeUiGTweener = UI.TweenScale(UIMgrConst.OpenUIAnimEffectScale, UIMgrConst.UIAnimEffectTime).SetIgnoreEngineTimeScale(true).SetEase(EaseType.BackIn).OnComplete(() => { 
            
                closeUiGTweener = null;
                onComplete?.Invoke();
            });
        }

        public override void Dispose()
        {
            UI.Dispose();
            UI = null;
        }
    }
}