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
    }


    public class SwapElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Swap;
        public ElementItem Tar { get; set; }

        public new bool IsPlay => base.IsPlay;
        
        public float Duration => duration;

        public Vector3 formPot = Vector3.zero;
        public Vector3 toPot = Vector3.zero;

        private float duration = 0.3f;
        public void SetElementAndPlay(ElementItem elementItem)
        {
            Tar = elementItem;
            Play();
        }

        protected override void AddTweenToSequence(Sequence sequence)
        {
            sequence.Append(DOTween.To(
                () => 0f,
                x =>
                {
                    Tar.Pos = Vector3.Lerp(formPot, toPot, x);
                },
                1f,
                duration));
        }

        public override void Disp()
        {
            base.Disp();
            Tar = null;
            formPot = Vector3.zero;
            toPot = Vector3.zero;
        }


    }

}

