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
            // ����Ĭ�϶���������������
            DOTween.defaultEaseType = Ease.Linear;
           
        }


    }

}
