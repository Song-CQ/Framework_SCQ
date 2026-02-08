/****************************************************
    文件: ClickBtn_DoTweenAni.cs
    作者: Clear
    日期: 2026/2/8 17:8:29
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace FutureCore
{
    public class ClickButton_DoTweenAni : MonoBehaviour
    {
        [TitleGroup("按钮点击动画")]
        [Header("参数")]
        [SerializeField] private float clickScale = 0.8f;    // 点击时的缩放比例
        [SerializeField] private float releaseScale = 1f;    // 释放时的缩放比例
        [SerializeField] private float clickDuration = 0.1f; // 点击动画时长
        [SerializeField] private float releaseDuration = 0.1f; // 释放动画时长
        [SerializeField] private Ease clickEase = Ease.Linear;    // 点击时的动画曲线
        [SerializeField] private Ease releaseEase = Ease.Linear;    // 释放时的动画曲线

        private Button button;
        private Vector3 originalScale;

        private void Awake()
        {
            button = GetComponent<Button>();
            originalScale = transform.localScale;

            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            // 创建点击动画序列
            Sequence clickSequence = DOTween.Sequence();

            // 点击时缩小
            clickSequence.Append(transform.DOScale(originalScale * clickScale, clickDuration)
                .SetEase(Ease.Linear));

            // 释放时恢复
            clickSequence.Append(transform.DOScale(originalScale * releaseScale, releaseDuration)
                .SetEase(Ease.Linear));

            // 播放动画
            clickSequence.Play();
        }
    }
}