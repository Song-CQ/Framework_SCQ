using DG.Tweening;
using UnityEngine;

namespace FutureCore
{
    public class DOTweenHelper
    {
        public static void Init()
        {
            IDOTweenInit doTweenInit = DOTween.Init();

            DOTween.SetTweensCapacity(1024, 1024);
            // 设置默认动画缓动曲线类型
            DOTween.defaultEaseType = Ease.Linear;
           
        }


    }

}
