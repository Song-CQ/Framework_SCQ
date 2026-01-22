using ConsoleE;
using DG.Tweening;
using FairyGUI;
using FutureCore;
using ProjectApp.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class ElementAni_System : BaseSystem
    {
        private Dictionary<ElementAniType, Queue<IElementAni>> animationLibrary = new Dictionary<ElementAniType, Queue<IElementAni>>();

        public override void Init()
        {
            base.Init();
            IsAutoRegisterUpdate = true;
        }

        public override void Start()
        {
            base.Start();
            
        }

        public override void Shutdown()
        {
            base.Shutdown();
            
        }

        public override void Run()
        {
            base.Run();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void PlaySwapAin(ElementItem item1,ElementItem item2)
        {
            //停止正在播放的Dotw

            SwapElementAni_Sequence ani1 = GetAnimation(ElementAniType.Swap) as SwapElementAni_Sequence;
            ani1.formPot = item1.Pos;
            ani1.toPot = item2.Pos;
            ani1.SetElementAndPlay(item1);

            SwapElementAni_Sequence ani2 = GetAnimation(ElementAniType.Swap) as SwapElementAni_Sequence;
            ani2.formPot = item1.Pos;
            ani2.toPot = item2.Pos;
            ani2.SetElementAndPlay(item2);




        }

        

        public IElementAni GetAnimation(ElementAniType type)
        {

            if (!animationLibrary.TryGetValue(type, out Queue<IElementAni> aniQueue))
            {
                aniQueue = new Queue<IElementAni>();
                animationLibrary[type] = aniQueue;
            }
            IElementAni ani = null;
            if (aniQueue.Count > 0)
            {
                ani = aniQueue.Dequeue();
            }
            else
            {
                ani = CreateElementAni(type);
            }          
            return ani;

        }

        private IElementAni CreateElementAni(ElementAniType type)
        {
            switch (type)
            {
                case ElementAniType.Swap:
                    return new SwapElementAni_Sequence();
                default:
                    return null;

            }
        }





        #region 核心属性
        [SerializeField, Range(0f, 2f)]
        private float globalPlaybackSpeed = 1f;

        public float GlobalPlaybackSpeed
        {
            get => globalPlaybackSpeed;
            set
            {
                globalPlaybackSpeed = Mathf.Clamp(value, 0f, 2f);
                UpdateAllTweensSpeed();
            }
        }

        private Dictionary<Tween, float> originalSpeeds = new Dictionary<Tween, float>();
        private List<Tween> managedTweens = new List<Tween>();
        private bool isPaused = false;
        #endregion

        #region 公共方法
        // 注册动画，使其受全局控制
        public Tween RegisterTween(Tween tween)
        {
            if (tween == null) return null;

            managedTweens.Add(tween);
            originalSpeeds[tween] = tween.timeScale;

            // 立即应用当前速度
            ApplySpeedToTween(tween);

            // 动画完成时自动注销
            tween.OnKill(() => UnregisterTween(tween));
            tween.OnComplete(() => UnregisterTween(tween));

            return tween;
        }

        // 创建并注册动画
        public Tween CreateControlledTween(Tween tween)
        {
            return RegisterTween(tween);
        }

        // 暂停所有动画
        public void PauseAll()
        {
            isPaused = true;
            foreach (var tween in managedTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    tween.Pause();
                }
            }
        }

        // 恢复所有动画
        public void ResumeAll()
        {
            isPaused = false;
            foreach (var tween in managedTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    tween.Play();
                }
            }
        }

        // 设置全局播放速度（0=暂停，0.5=慢速，1=正常，2=快速）
        public void SetGlobalSpeed(float speed)
        {
            GlobalPlaybackSpeed = speed;
        }

        // 获取当前活跃动画数量
        public int GetActiveTweenCount()
        {
            return managedTweens.Count;
        }
        #endregion

        #region 私有方法
        private void UpdateAllTweensSpeed()
        {
            foreach (var tween in managedTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    ApplySpeedToTween(tween);
                }
            }
        }

        private void ApplySpeedToTween(Tween tween)
        {
            if (originalSpeeds.TryGetValue(tween, out float originalSpeed))
            {
                if (globalPlaybackSpeed == 0f)
                {
                    tween.Pause();
                }
                else
                {
                    if (!isPaused)
                    {
                        tween.Play();
                    }
                    tween.timeScale = originalSpeed * globalPlaybackSpeed;
                }
            }
        }

        private void UnregisterTween(Tween tween)
        {
            if (tween != null)
            {
                managedTweens.Remove(tween);
                originalSpeeds.Remove(tween);
            }
        }

        private void Update()
        {
            // 每帧清理无效的动画
            for (int i = managedTweens.Count - 1; i >= 0; i--)
            {
                var tween = managedTweens[i];
                if (tween == null || !tween.IsActive())
                {
                    managedTweens.RemoveAt(i);
                    if (tween != null)
                    {
                        originalSpeeds.Remove(tween);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            // 清理所有动画
            foreach (var tween in managedTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    tween.Kill();
                }
            }
            managedTweens.Clear();
            originalSpeeds.Clear();
        }
        #endregion



    }
}
