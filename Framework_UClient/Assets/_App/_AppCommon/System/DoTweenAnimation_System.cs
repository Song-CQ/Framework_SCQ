using DG.Tweening;
using FutureCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class DoTweenAnimation_System : BaseSystem
    {
        private Dictionary<string, Sequence> animationLibrary = new Dictionary<string, Sequence>();

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

        public Transform target;
        public float duration = 1f;

        public void PlayComplexAnimation()
        {
            // 创建一个动画序列
            Sequence sequence = DOTween.Sequence();

            // ========== 第一阶段：顺序动画 ==========

            // 1. 先移动到位置A
            sequence.Append(target.DOMove(new Vector3(2, 0, 0), duration));

            // 2. 然后旋转90度
            sequence.Append(target.DORotate(new Vector3(0, 90, 0), duration));

            // ========== 第二阶段：同时播放的动画 ==========

            // 3. 移动到位置B的同时改变颜色（假设有Renderer）
            sequence.Append(target.DOMove(new Vector3(2, 2, 0), duration));
            sequence.Join(GetComponent<Renderer>().material.DOColor(Color.red, duration));

            // 4. 然后缩放和旋转同时进行
            sequence.Append(target.DOScale(Vector3.one * 2, duration));
            sequence.Join(target.DORotate(new Vector3(0, 180, 0), duration));

            // ========== 第三阶段：回调和其他控制 ==========

            // 动画完成时回调
            sequence.OnComplete(() =>
            {
                Debug.Log("动画播放完成！");
            });

            // 动画开始时回调
            sequence.OnStart(() =>
            {
                Debug.Log("动画开始播放！");
            });

            // 设置循环
            sequence.SetLoops(2, LoopType.Yoyo); // 来回播放2次

            // 设置缓动函数
            sequence.SetEase(Ease.OutBounce);

            // 播放动画
            sequence.Play();
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
