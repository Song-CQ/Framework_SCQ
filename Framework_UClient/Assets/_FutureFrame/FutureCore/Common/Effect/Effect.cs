/****************************************************
    文件: pr.cs
    作者: Clear
    日期: 2023/11/28 15:58:7
    类型: 框架核心脚本(请勿修改)
    功能: 特效
*****************************************************/
using System;
using UnityEngine;

namespace FutureCore
{
    public class Effect
    {
        public EffectEntity entity;

        public EffectData data;

        private bool isPlay = false;

        /// <summary>
        /// 结束CallBack
        /// </summary>
        public event Action<Effect> Event_Stop_Action;

        public Effect()
        {
            
        }
        public Effect(EffectData effectData, EffectEntity effectEntity)
        {
            this.data = effectData;
            this.entity = effectEntity;
        }

        public void Play()
        {
            isPlay = true;
            entity.SetActive(true);
            StopType stopType = data.stopType;

            if (entity.main_ParticleSystem != null)
            {
                entity.main_ParticleSystem.Play();        
            }
            else if(stopType == StopType.ParticleSystemStopped_ToMain)
            {
                stopType = StopType.Default;
            }

            switch (stopType)
            {
                case StopType.ParticleSystemStopped_ToMain:
                    entity.Event_OnParticleSystemStopped += Stopped;
                    break;
                case StopType.Default:
                default:
                    if (data.lifeTime != -1)
                    {
                        TimerUtil.Simple.AddTimer(data.lifeTime,Stopped);
                    }
                    break;
            }




        }

        public void Stopped()
        {
            if (!isPlay)
            {
                return;
            }
            isPlay = false;
            if (entity.main_ParticleSystem != null)
            {
                entity.Event_OnParticleSystemStopped -= Stopped;
            }

            entity.SetActive(false);
            Event_Stop_Action?.Invoke(this);
        }


    }
}