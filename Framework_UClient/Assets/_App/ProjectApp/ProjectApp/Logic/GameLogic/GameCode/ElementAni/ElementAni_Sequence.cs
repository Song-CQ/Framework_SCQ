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
        Swap = 0,
        Clear,
        FallMove,
    }

    public interface IElementAni
    {
        ElementAniType Key { get; }
        ElementItem Tar { get; }
        bool IsPlay { get; }
        float Duration { get; }

        void SetElementAndPlay(ElementItem elementItem);

        void Pause();
        void CanlePause();
        void Stop();
        void ResetState();

    }


    public class SwapElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Swap;
        public ElementItem Tar { get; set; }

        public new bool IsPlay => base.IsPlay;

        public float Duration { get; private set; } = 0.3f;

        public Vector3 formPot = Vector3.zero;
        public Vector3 toPot = Vector3.zero;
        protected override void OnStart()
        {
            base.OnStart();
            Tar.Pos = formPot;

        }


        public void SetElementAndPlay(ElementItem elementItem)
        {
            Tar = elementItem;
            Play();
        }

        protected override void AddTweenToSequence(Sequence seq)
        {
            seq.Append(DOTween.To(
                () => 0f,
                x =>
                {
                    Tar.Pos = Vector3.Lerp(formPot, toPot, x);
                },
                1f,
                Duration));
        }

        public override void Disp()
        {
            base.Disp();
            Tar = null;
            formPot = Vector3.zero;
            toPot = Vector3.zero;
        }

        public void ResetState()
        {
            
        }
    }

    public class FallMoveElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Swap;
        public ElementItem Tar { get; set; }

        public new bool IsPlay => base.IsPlay;

        public float Duration => fallDuration+fallBounceDuration;

        //[Header("下落设置")]
        private float fallDuration = 0.4f;      // 下落持续时间
        private float fallBounceDuration = 0.7f; // 弹跳持续时间
        private float bounceHeight = 0.2f;      // 弹跳高度
        private Ease fallEase = Ease.OutCubic;  // 下落缓动
        private Ease bounceEase = Ease.OutBounce; // 弹跳缓动
        private Vector3 bouncePoint;

        public Vector3 formPot;  // 起始位置
        public Vector3 toPot;    // 目标位置

        public void SetElementAndPlay(ElementItem elementItem)
        {
            Tar = elementItem;
            Play();
        }
        protected override void OnStart()
        {
            base.OnStart();
            Tar.Pos = formPot;
            // 创建弹跳中间点
            bouncePoint = new Vector3(
                toPot.x,
                toPot.y + bounceHeight,
                toPot.z
            );
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

        public override void ResetState()
        {
            
        }
    }

    public class ClearElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key =>  ElementAniType.Swap;
        public ElementItem Tar { get; set; }

        public new bool IsPlay => base.IsPlay;

        public float Duration { get; private set; } = 0.04f;

        public void SetElementAndPlay(ElementItem elementItem)
        {
            Tar = elementItem;
            Play();
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

        public override void ResetState()
        {
            Tar.Transform.localScale = Vector3.one;
        }
        protected override void OnComplete()
        {
            base.OnComplete();
            ResetState();

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

            effect.entity.transform.position = Tar.Transform.position - Vector3.forward;
            effect.entity.transform.localScale = Vector3.one * 0.3f;
            effect.Play();

           

        }
    }
}

