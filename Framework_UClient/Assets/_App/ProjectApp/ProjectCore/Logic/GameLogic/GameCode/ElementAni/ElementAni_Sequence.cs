using DG.Tweening;
using FutureCore;
using System;
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
        bool IsRun { get; }
        bool IsComplete { get; }
        float Duration { get; }

        /// <summary>
        /// 开始播放时间
        /// </summary>
        float StartPlayTime { get; }

        void SetElement(ElementItem elementItem, float delay);

        void Play();
        void Pause();
        void CanlePause();
        void Stop();
        void Run();
    }


    public class MoveElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Move;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsRun => base.IsRun;
        bool IElementAni.IsComplete => base.IsComplete;

        float IElementAni.Duration => base.Duration;

        public float Delay;

        float IElementAni.StartPlayTime => base.StartPlayTime;



        public Vector3 formPot = Vector3Int.zero;
        public Vector3 toPot = Vector3Int.zero;

        private void SetDuration()
        {
            Duration = 0.3f;
        }

        public void SetElement(ElementItem elementItem, float delay)
        {
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime() + delay;
            Delay = delay;
            SetDuration();


        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            SetDuration();
            seq.Append(DOTween.To(
                () => 0f,

                x =>
                {
                    Tar.Pos = Vector3.Lerp(formPot, toPot, x);
                    Debug.Log(TimerUtil.GetGameTime() + Tar.Data.ToString() + "要移" + formPot + " to " + toPot + " x " + x + "当前" + Tar.Pos);
                },
                1f,
                Duration));
        }

        protected override void OnComplete()
        {
            base.OnComplete();

            Tar.Pos = toPot;
            Debug.Log(Tar.Data.ToString() + "移动完成" + formPot + " to " + toPot + "当前" + Tar.Pos);

        }

        protected override void ResetState()
        {
            base.ResetState();
            Tar = null;
            Delay = 0;
          

            formPot = Vector3Int.zero;
            toPot = Vector3Int.zero;
        }




    }



    public class FallMoveElementAni_Sequence : Base_CurveTween, IElementAni
    {
        public ElementAniType Key => ElementAniType.FallMove;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsComplete => base.IsComplete;
        bool IElementAni.IsRun => base.IsRun;

        float IElementAni.Duration => base.Duration;


        public float Delay;

        float IElementAni.StartPlayTime => base.StartPlayTime;


        //[Header("下落设置")]
        private float fallDuration = 0.3f;      // 下落持续时间
        private float fallBounceDuration = 0.7f; // 弹跳持续时间
        private float bounceHeight = 0.2f;      // 弹跳高度
        private Ease fallEase = Ease.OutCubic;  // 下落缓动
        private Ease bounceEase = Ease.OutBounce; // 弹跳缓动
        private Vector3 bouncePoint;

        private float fallEnd;
        private float fallBounceEnd;
        public Vector3 formPot;  // 起始位置
        public Vector3 toPot;    // 目标位置


        public AnimationCurve moveCurve1;
        public AnimationCurve moveCurve2;
        public FallMoveElementAni_Sequence()
        {
            moveCurve1 = fallEase.ToAnimationCurve();
            moveCurve2 = bounceEase.ToAnimationCurve();
        }

        public void SetElement(ElementItem elementItem, float delay)
        {
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime() + delay;
            Delay = delay;
            Tar.Pos = formPot;
            // 创建弹跳中间点
            bouncePoint = new Vector3(
                toPot.x,
                toPot.y + bounceHeight,
                toPot.z
            );

            fallDuration = 0.3f;
            fallDuration += Vector2.Distance(formPot, toPot) / 10 * 0.04f;

            //Debug.Log("fallDuration"+ fallDuration);
            fallEnd = GetMoveCurveToTime(fallDuration);
            fallBounceEnd = 1;
            Duration = fallDuration + fallBounceDuration;
        }
        protected override void OnStart()
        {
            base.OnStart();

        }

        protected void AddTweenToSequence(Sequence seq)
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



        protected override void UpdateProgress(float curveValue)
        {

            if (curveValue <= fallEnd)
            {
                float x = moveCurve1.Evaluate(curveValue / fallEnd);
                x = Mathf.Clamp01(x);

                // 计算当前位置（从起始点下落到弹跳点）
                float currentY = Mathf.Lerp(formPot.y, bouncePoint.y, x);
                Tar.Pos = new Vector3(
                    Mathf.Lerp(formPot.x, bouncePoint.x, x),
                    currentY,
                    Mathf.Lerp(formPot.z, bouncePoint.z, x)
                );

            }
            else if (curveValue <= fallBounceEnd)
            {
                float x = moveCurve2.Evaluate((curveValue - fallEnd) / (fallBounceEnd - fallEnd));
                x = Mathf.Clamp01(x);
                Tar.Pos = Vector3.Lerp(bouncePoint, toPot, x);
            }







        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Tar.Pos = toPot;

        }

        protected override void ResetState()
        {
            base.ResetState();
            Delay = 0f;
            Tar = null;
            fallEnd = 0;
            fallBounceEnd = 0;
            formPot = Vector3.zero;  // 起始位置
            toPot = Vector3.zero;
        }

    }

    public class ClearElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Clear;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsComplete => base.IsComplete;
        bool IElementAni.IsRun => base.IsRun;

        float IElementAni.Duration => base.Duration;

        public float Delay;

        float IElementAni.StartPlayTime => base.StartPlayTime;

        private void SetDuration()
        {
            Duration = 0.02f;
        }

        public void SetElement(ElementItem elementItem, float delay)
        {
            SetDuration();
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime() + delay;
            Delay = delay;    
        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            SetDuration();
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

            effect.entity.transform.position = Tar.Transform.position - Vector3.forward * 10;
            effect.entity.transform.localScale = Vector3.one * 5;
            effect.Play();



        }

        protected override void ResetState()
        {
            base.ResetState();
            Delay = 0f;
            Tar = null;
        }
    }

    public class ElasticShakeAnimation_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.ElasticShake;
        public ElementItem Tar { get; set; }

        bool IElementAni.IsPlay => base.IsPlay;
        bool IElementAni.IsComplete => base.IsComplete;
        bool IElementAni.IsRun => base.IsRun;
        float IElementAni.Duration => base.Duration;


        float IElementAni.StartPlayTime => base.StartPlayTime;


        public Vector3 originalPos;
        public float Delay;



        private float shakeDuration = 0.6f;
        private float shakeIntensity = 1.2f;
        private int bounces = 3;//次数


        private void SetDuration()
        {
            Duration = shakeDuration;
        }

        public void SetElement(ElementItem elementItem, float delay)
        {
            SetDuration();
            Tar = elementItem;
            StartPlayTime = TimerUtil.GetGameTime() + delay;
            Delay = delay;


        }

        protected override void OnStart()
        {
            base.OnStart();
            Debug.Log(Tar.Data.ToString() + "开始抖动" + TimerUtil.GetGameTime());

        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            SetDuration();

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
                    //Debug.Log(Tar.Data.ToString() + "正在抖动" + TimerUtil.GetGameTime() + Tar.Pos);
                },
                1f,
                shakeDuration
            ).SetEase(Ease.OutSine));

        }


        protected override void OnComplete()
        {
            base.OnComplete();
            Debug.Log(Tar.Data.ToString() + "抖动结束" + TimerUtil.GetGameTime() + Tar.Pos + "目标" + originalPos);
            // Tar.Pos = originalPos;
        }
        private Vector3 GetRandomDirection()
        {
            // 基于元素ID生成固定的随机方向
            int elementId = Tar.GetHashCode();
            float angle = (elementId % 360) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
        }


        protected override void ResetState()
        {
            base.ResetState();
            Delay = 0;
            originalPos = Vector3.zero;


        }


    }
}

