using Codice.Client.Common;
using DG.Tweening;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FutureCore
{
   
    public static class DOTweenHelper
    {
        public static void Init()
        {
            IDOTweenInit doTweenInit = DOTween.Init();

            DOTween.SetTweensCapacity(1024, 1024);
            // ����Ĭ�϶���������������
            DOTween.defaultEaseType = Ease.Linear;

            DOTween.logBehaviour = LogBehaviour.Default;
        }
        public static Sequence CraidSequence()
        {
            // 创建一个动画序列
            Sequence sequence = DOTween.Sequence();

            return sequence;




        }
        public static  void PlayComplexAnimation(Transform target,float duration)
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
            //sequence.Join(GetComponent<Renderer>().material.DOColor(Color.red, duration));

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


    }

}
