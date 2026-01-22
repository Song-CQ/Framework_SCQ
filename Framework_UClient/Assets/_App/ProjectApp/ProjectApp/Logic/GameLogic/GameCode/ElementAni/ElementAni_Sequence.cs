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
        public ElementAniType Key { get; }
        public ElementItem Tar { get; }
        public void SetElementAndPlay(ElementItem elementItem);
    }


    public class SwapElementAni_Sequence : DoTweenSequence, IElementAni
    {
        public ElementAniType Key => ElementAniType.Swap;
        public ElementItem Tar { get; set; }
        public Vector3 formPot = Vector3.zero;
        public Vector3 toPot = Vector3.zero;

        private float time = 0.3f;
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
                time));
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

