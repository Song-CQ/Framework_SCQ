/****************************************************
    文件: UI_EffectManager.cs
    作者: Clear
    日期: 2023/11/23 16:8:25
    类型: 框架核心脚本(请勿修改)
    功能: UI特效管理器
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class UI_EffectManager :BaseMonoMgr<UI_EffectManager>
    {
        private Dictionary<string,ObjectPool<Effect>> uiEffectPool = new Dictionary<string, ObjectPool<Effect>>();
        private FXEffectUICtrl fXEffectUICtrl;

        private Transform UIEffectTrf;
        public override void Init()
        {
            base.Init();
            UIEffectTrf = new GameObject("[UIEffectPool]").transform;
            UIEffectTrf.parent = AppObjConst.UICacheGo.transform;
            UIEffectTrf.localPosition = Vector3.zero;
        }

        public override void StartUp()
        {
            base.StartUp();

            AppDispatcher.Instance.AddOnceListener(AppMsg.App_StartUp, OpenUI_Effect);
        }

        private void OpenUI_Effect(object obj)
        {         
            fXEffectUICtrl = ModuleMgr.Instance.GetUICtrl(UICtrlConst.FXEffectUICtrl) as FXEffectUICtrl;
            fXEffectUICtrl.OpenUI();
            InputMgr.ClickScreen += InputMgr_ClickScreen;

        }

        private void InputMgr_ClickScreen(Vector2 obj)
        {
            Effect uiEffect =  GetSimpleUIEffect("testSys");

            fXEffectUICtrl.PlayEffect(uiEffect,obj);
        }

        private Effect GetSimpleUIEffect(string name)
        {
            if (!uiEffectPool.ContainsKey(name))
            {
                uiEffectPool[name] = new ObjectPool<Effect>(() => {
                    EffectEntity effectEntity = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>(name)).GetComponent<EffectEntity>();
                    effectEntity.SetActive(false);
                    EffectData effectData = new EffectData();
                    effectData.stopType = StopType.ParticleSystemStopped_ToMain;
                    effectData.effectName = name;
                    effectData.effectPath = name;
                    Effect effect = new Effect(effectData, effectEntity);
                    effect.Event_Stop_Action += Effect_Event_Stop_Action;
                    return effect;
                });
                
            }

            var poll = uiEffectPool[name];
            
            return poll.Get();
        }

        private void Effect_Event_Stop_Action(Effect effect)
        {
            if (uiEffectPool.ContainsKey(effect.data.effectName))
            {
                uiEffectPool[effect.data.effectName].Release(effect);
            }

            effect.entity.transform.SetParent(UIEffectTrf);
        }
    }
}