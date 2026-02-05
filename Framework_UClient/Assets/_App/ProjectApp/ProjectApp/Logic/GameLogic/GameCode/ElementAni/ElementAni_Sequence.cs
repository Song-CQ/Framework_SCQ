using Codice.Client.Common;
using DG.DemiEditor;
using DG.Tweening;
using FutureCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp.GameLogic
{
    public enum ElementAniType
    {
        Move = 0,
        Clear,
        FallMove,
        ElasticShake,
    }

    public interface IElementAni
    {
        ElementAniType Key { get; }
        ElementItem Tar { get; }
        bool IsPlay { get; }
        bool IsComplete { get; }
        float Duration { get; }
        /// <summary>
        /// 延迟播放时间
        /// </summary>
        float Delay { get; }
        
        /// <summary>
        /// 开始播放时间
        /// </summary>
        float StartPlayTime { get;}

        void SetElement(ElementItem elementItem,float delay);

        void Play();
        void Pause();
        void CanlePause();
        void Stop();

        void ResetState();



    }


    public class MoveElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Move;
        public ElementItem Tar { get; set; }

        public new bool IsPlay => base.IsPlay;

        public float Duration { get; private set; } = 0.3f;

        public float Delay { get; set; } 

        public float StartPlayTime  { get; private set; }

        bool IElementAni.IsComplete => base.IsComplete;

        public Vector3 formPot = Vector3Int.zero;
        public Vector3 toPot = Vector3Int.zero;
        protected override void OnStart()
        {
            base.OnStart();
           
        }


        public void SetElement(ElementItem elementItem,float delay)
        {
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime();
            Delay = delay;

         
        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            seq.Append(DOTween.To(
                () => 0f,
        
                x =>
                {
                    Tar.Pos = Vector3.Lerp(formPot, toPot, x);
                    Debug.Log(TimerUtil.GetGameTime()+Tar.Data.ToString() + "要移" + formPot + " to " + toPot+ " x "+x + "当前"+ Tar.Pos);
                },
                1f,
                Duration));
        }

        protected override void OnComplete()
        {
            base.OnComplete();

            Tar.Pos = toPot;
            Debug.Log(Tar.Data.ToString() + "移动完成" + formPot + " to " + toPot + "当前"+ Tar.Pos);

        }
        

        


        public override void Disp()
        {
            base.Disp();
            Tar = null;
            Delay = 0;
            StartPlayTime = 0;
            Duration = 0;


            formPot = Vector3Int.zero;
            toPot = Vector3Int.zero;
        }





    }

    public class FallMoveElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.FallMove;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsComplete => base.IsComplete;

        public float Duration => fallDuration + fallBounceDuration;

        public float Delay { get; set; }

        public float StartPlayTime  { get; set; }

        //[Header("下落设置")]
        private float fallDuration = 0.4f;      // 下落持续时间
        private float fallBounceDuration = 0.7f; // 弹跳持续时间
        private float bounceHeight = 0.2f;      // 弹跳高度
        private Ease fallEase = Ease.OutCubic;  // 下落缓动
        private Ease bounceEase = Ease.OutBounce; // 弹跳缓动
        private Vector3 bouncePoint;

        public Vector3 formPot;  // 起始位置
        public Vector3 toPot;    // 目标位置

        public void SetElement(ElementItem elementItem,float delay)
        {
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime();
            Delay = delay;

            Tar.Pos = formPot;
            // 创建弹跳中间点
            bouncePoint = new Vector3(
                toPot.x,
                toPot.y + bounceHeight,
                toPot.z
            );

        }
        protected override void OnStart()
        {
            base.OnStart();
            Tar.Pos = toPot;
        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            // 第一阶段：下落动画
            seq.Append(DOTween.To(
                () => 0f,
                x =>
                {
                    // 计算当前位置（从起始点下落到弹跳点）
                    float currentY = Mathf.Lerp(formPot.y, bouncePoint.y, x);
                    Tar.Pos = new Vector3(
                        Mathf.Lerp(formPot.x, bouncePoint.x, x),
                        currentY,
                        Mathf.Lerp(formPot.z, bouncePoint.z, x)
                    );
                },
                1f,
                fallDuration
            ).SetEase(fallEase));

            // 第二阶段：弹跳动画
            seq.Append(DOTween.To(
                () => 0f,
                x =>
                {
                    Tar.Pos = Vector3.Lerp(bouncePoint, toPot, x);
                },
                1f,
                fallBounceDuration
            ).SetEase(bounceEase));

        }

        protected override void OnComplete()
        {
            base.OnComplete();


        }



    }

    public class ClearElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key =>  ElementAniType.Clear;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsComplete => base.IsComplete;

        public float Duration { get; private set; } = 0.04f;

        public float Delay {set;get;}

        public float StartPlayTime  {set;get;}

        public void SetElement(ElementItem elementItem,float delay)
        {
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime();
            Delay = delay;
        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            seq.Append(DOTween.To(
               () => 1f,
               x =>
               {
                   Tar.Transform.localScale = Vector3.one * (x);
               },
               1.2f,
               Duration).SetEase(Ease.InBack));
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            
            Tar.Transform.localScale = Vector3.one;

            string effectName = "ClickUIEffect";
            string effectPath = "Prefabs/Effect/Common_UIEffect/ClickUIEffect";
            //这块可以用异步加载
            EffectEntity effectEntity = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>(effectPath)).GetComponent<EffectEntity>();

            EffectData effectData = new EffectData();
            effectData.stopType = StopType.ParticleSystemStopped_ToMain;
            effectData.effectName = effectName;
            effectData.effectPath = effectPath;
            Effect effect = new Effect(effectData, effectEntity);
            effect.autoDestroy = true;

            effect.entity.transform.position = Tar.Transform.position - Vector3.forward*10;
            effect.entity.transform.localScale = Vector3.one * 5;
            effect.Play();

           

        }
    }


    public class ElasticShakeAnimation_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.ElasticShake;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsComplete => base.IsComplete;

        public float Duration => shakeDuration;

        public float Delay {set;get;}

        public float StartPlayTime  {set;get;}

        private float shakeDuration = 0.6f;
        private float shakeIntensity = 0.05f;
        private int bounces = 3;
        public Vector3 originalPos;

        public void SetElement(ElementItem elementItem,float delay)
        {
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime();
            Delay = delay;
        }

        protected override void OnStart()
        {
            base.OnStart();
            
         
        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            seq.Append(DOTween.To(
                () => 0f,
                progress =>
                {
                    // 弹性公式：y = e^(-damping * t) * sin(frequency * t)
                    float damping = 5f;
                    float frequency = Mathf.PI * 2 * bounces;
                    float time = progress * shakeDuration;

                    float shakeValue = Mathf.Exp(-damping * time) *
                                      Mathf.Sin(frequency * time) *
                                      shakeIntensity;

                    // 随机方向抖动
                    Vector3 direction = GetRandomDirection();
                    Tar.Pos = originalPos + direction * shakeValue;
                    
                },
                1f,
                shakeDuration
            ).SetEase(Ease.OutSine));

        }


        protected override void OnComplete()
        {
            base.OnComplete();
            Tar.Pos = originalPos;
        }
        private Vector3 GetRandomDirection()
        {
            // 基于元素ID生成固定的随机方向
            int elementId = Tar.GetHashCode();
            float angle = (elementId % 360) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
        }

    }
}

