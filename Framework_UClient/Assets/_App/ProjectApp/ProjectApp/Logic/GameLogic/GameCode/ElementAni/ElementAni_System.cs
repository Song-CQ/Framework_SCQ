using ConsoleE;
using DG.Tweening;
using FairyGUI;
using FutureCore;
using ProjectApp.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace ProjectApp
{
    public class ElementAni_System : BaseSystem
    {
        private Dictionary<ElementAniType, Queue<IElementAni>> animationLibrary = new Dictionary<ElementAniType, Queue<IElementAni>>();
        private Dictionary<ElementItem,IElementAni> runAllElementAni; 
        private List<ElementItem> elementItems;
 
        public override void Init()
        {
            base.Init();
            IsAutoRegisterUpdate = true;
            animationLibrary = new Dictionary<ElementAniType, Queue<IElementAni>>();
            runAllElementAni = new Dictionary<ElementItem, IElementAni>();
            elementItems = new List<ElementItem>();
        }

        public override void Start()
        {
            base.Start();

            foreach (var item in runAllElementAni)
            {
                item.Value.CanlePause();
            }
            
        }

        public override void Shutdown()
        {
            base.Shutdown();
            foreach (var item in runAllElementAni)
            {
                item.Value.Pause();
            }
            
        }

        public override void Run()
        {
            base.Run();
            
            elementItems.Clear();
            foreach (var item in runAllElementAni)
            {
                var elementAni = item.Value;

                if(elementAni.IsPlay)
                {
                    continue;
                }
                //回收对象
                ReleaseIElementAni(elementAni);

                //加入移除列表
                elementItems.Add(item.Key);
            }

            foreach (var item in elementItems)
            {
                runAllElementAni.Remove(item);      
            } 
        }

        public override void Dispose()
        {
            base.Dispose();
        }




        private void AddRunElementAni(ElementItem item,IElementAni ani)
        {
            if (item != null && ani != null) return;
            runAllElementAni[item] = ani;
        }


        private void StopElementItemAni(ElementItem item)
        {
            if (runAllElementAni.TryGetValue(item,out IElementAni elementAni))
            {
                runAllElementAni.Remove(item);
                ReleaseIElementAni(elementAni);
            }

        }

 
        private IElementAni GetAnimation(ElementAniType type)
        {

            if (!animationLibrary.TryGetValue(type, out Queue<IElementAni> aniQueue))
            {
                aniQueue = new Queue<IElementAni>();
                animationLibrary[type] = aniQueue;
            }
            IElementAni ani = null;
            if (aniQueue.Count > 0)
            {
                //Debug.Log("使用旧的");
                ani = aniQueue.Dequeue();
            }
            else
            {
                //Debug.Log("创建新达到");
                ani = CreateElementAni(type);
            }          
            return ani;

        }
        
        private void ReleaseIElementAni(IElementAni val)
        {
            val.Stop();
            if(!animationLibrary.ContainsKey(val.Key))
            {
                animationLibrary = new Dictionary<ElementAniType, Queue<IElementAni>>();
            }
            animationLibrary[val.Key].Enqueue(val);
        }
        private IElementAni CreateElementAni(ElementAniType type)
        {
            switch (type)
            {
                case ElementAniType.Swap:
                    return new SwapElementAni_Sequence(); 
                case ElementAniType.Clear:
                    return new ClearElementAni_Sequence();
                case ElementAniType.FallMove:          
                    return new FallMoveElementAni_Sequence();
                default:
                    return null;

            }
        }


        #region 动画
       
        public float PlayAin_SwapElement(ElementItem item1, ElementItem item2)
        {
            //停止正在播放的Dotw
            StopElementItemAni(item1);
            StopElementItemAni(item2);


            SwapElementAni_Sequence ani1 = GetAnimation(ElementAniType.Swap) as SwapElementAni_Sequence;
            ani1.formPot = item1.Pos;
            ani1.toPot = item2.Pos;
            ani1.SetElementAndPlay(item1);
            AddRunElementAni(item1,ani1);

            SwapElementAni_Sequence ani2 = GetAnimation(ElementAniType.Swap) as SwapElementAni_Sequence;
            ani2.formPot = item2.Pos;
            ani2.toPot = item1.Pos;
            ani2.SetElementAndPlay(item2);
            AddRunElementAni(item2,ani2);

            float dur = ani1.Duration;


            return dur;
        }
        public float PlayAin_ClearElements(List<ElementItem> items)
        {
            float dur = 0;
            foreach (var item in items)
            {
                if (item == null) continue;
                StopElementItemAni(item);

                IElementAni ani = GetAnimation(ElementAniType.Clear);
                ani.SetElementAndPlay(item);

                AddRunElementAni(item,ani);
                dur = ani.Duration;
            }
            return dur;
        }
        public float PlayAin_FallElements(List<ElementItem> elementItemList, List<Vector3> tarPotList)
        {
            float dur = 0;
            for (int i = 0; i < elementItemList.Count; i++)
            {
                var item = elementItemList[i];
                var pot = tarPotList[i];
                if (item == null) continue;
                StopElementItemAni(item);
                FallMoveElementAni_Sequence ani = GetAnimation(ElementAniType.FallMove) as FallMoveElementAni_Sequence;
                ani.formPot = item.Pos;
                ani.toPot = pot;
                
                ani.SetElementAndPlay(item);
                AddRunElementAni(item, ani);
                dur = ani.Duration;

            }

            return dur;
        }

        #endregion

        #region 测试方法
        /*
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


        */
        #endregion

    }
}
