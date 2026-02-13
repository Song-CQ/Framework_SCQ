using DG.Tweening;using System;using UnityEngine;using static UnityEngine.GraphicsBuffer;namespace FutureCore{       public static class DOTweenHelper    {        public static void Init()        {            IDOTweenInit doTweenInit = DOTween.Init();            DOTween.SetTweensCapacity(1024, 1024);            // ����Ĭ�϶���������������            DOTween.defaultEaseType = Ease.Linear;            DOTween.logBehaviour = LogBehaviour.Default;        }        public static Sequence CraidSequence()        {            // 创建一个动画序列            Sequence sequence = DOTween.Sequence();            return sequence;        }        public static  void PlayComplexAnimation(Transform target,float duration)        {            // 创建一个动画序列            Sequence sequence = DOTween.Sequence();            // ========== 第一阶段：顺序动画 ==========            // 1. 先移动到位置A            sequence.Append(target.DOMove(new Vector3(2, 0, 0), duration));            // 2. 然后旋转90度            sequence.Append(target.DORotate(new Vector3(0, 90, 0), duration));            // ========== 第二阶段：同时播放的动画 ==========            // 3. 移动到位置B的同时改变颜色（假设有Renderer）            sequence.Append(target.DOMove(new Vector3(2, 2, 0), duration));            //sequence.Join(GetComponent<Renderer>().material.DOColor(Color.red, duration));            // 4. 然后缩放和旋转同时进行            sequence.Append(target.DOScale(Vector3.one * 2, duration));            sequence.Join(target.DORotate(new Vector3(0, 180, 0), duration));            // ========== 第三阶段：回调和其他控制 ==========            // 动画完成时回调            sequence.OnComplete(() =>            {                Debug.Log("动画播放完成！");            });            // 动画开始时回调            sequence.OnStart(() =>            {                Debug.Log("动画开始播放！");            });            // 设置循环            sequence.SetLoops(2, LoopType.Yoyo); // 来回播放2次            // 设置缓动函数            sequence.SetEase(Ease.OutBounce);            // 播放动画            sequence.Play();        }        #region 动画曲线
        public static AnimationCurve ToAnimationCurve(this Ease ease, int samples = 20)
        {
            switch (ease)
            {
                case Ease.Linear:
                    return AnimationCurve.Linear(0, 0, 1, 1);

                case Ease.InSine:
                    return InSineToCurve(samples);
                case Ease.OutSine:
                    return OutSineToCurve(samples);
                case Ease.InOutSine:
                    return InOutSineToCurve(samples);

                case Ease.InQuad:
                    return InQuadToCurve(samples);
                case Ease.OutQuad:
                    return OutQuadToCurve(samples);
                case Ease.InOutQuad:
                    return InOutQuadToCurve(samples);

                case Ease.InCubic:
                    return InCubicToCurve(samples);
                case Ease.OutCubic:
                    return OutCubicToCurve(samples);
                case Ease.InOutCubic:
                    return InOutCubicToCurve(samples);

                case Ease.InQuart:
                    return InQuartToCurve(samples);
                case Ease.OutQuart:
                    return OutQuartToCurve(samples);
                case Ease.InOutQuart:
                    return InOutQuartToCurve(samples);

                case Ease.InQuint:
                    return InQuintToCurve(samples);
                case Ease.OutQuint:
                    return OutQuintToCurve(samples);
                case Ease.InOutQuint:
                    return InOutQuintToCurve(samples);

                case Ease.InExpo:
                    return InExpoToCurve(samples);
                case Ease.OutExpo:
                    return OutExpoToCurve(samples);
                case Ease.InOutExpo:
                    return InOutExpoToCurve(samples);

                case Ease.InCirc:
                    return InCircToCurve(samples);
                case Ease.OutCirc:
                    return OutCircToCurve(samples);
                case Ease.InOutCirc:
                    return InOutCircToCurve(samples);

                case Ease.InElastic:
                    return InElasticToCurve(samples);
                case Ease.OutElastic:
                    return OutElasticToCurve(samples);
                case Ease.InOutElastic:
                    return InOutElasticToCurve(samples);

                case Ease.InBack:
                    return InBackToCurve(samples);
                case Ease.OutBack:
                    return OutBackToCurve(samples);
                case Ease.InOutBack:
                    return InOutBackToCurve(samples);

                case Ease.InBounce:
                    return InBounceToCurve(samples);
                case Ease.OutBounce:
                    return OutBounceToCurve(samples);
                case Ease.InOutBounce:
                    return InOutBounceToCurve(samples);

                case Ease.Flash:
                case Ease.InFlash:
                case Ease.OutFlash:
                case Ease.InOutFlash:
                    return FlashToCurve(samples, ease);

                // DOTween 特有
                case Ease.INTERNAL_Custom:
                case Ease.Unset:
                default:
                    return AnimationCurve.Linear(0, 0, 1, 1);
            }
        }

        // Sine 函数
        private static float InSine(float x) => 1 - Mathf.Cos((x * Mathf.PI) / 2);
        private static float OutSine(float x) => Mathf.Sin((x * Mathf.PI) / 2);
        private static float InOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1) / 2;

        private static AnimationCurve InSineToCurve(int samples) => CreateCurve(samples, InSine);
        private static AnimationCurve OutSineToCurve(int samples) => CreateCurve(samples, OutSine);
        private static AnimationCurve InOutSineToCurve(int samples) => CreateCurve(samples, InOutSine);

        // Quad 函数
        private static float InQuad(float x) => x * x;
        private static float OutQuad(float x) => 1 - (1 - x) * (1 - x);
        private static float InOutQuad(float x) => x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;

        private static AnimationCurve InQuadToCurve(int samples) => CreateCurve(samples, InQuad);
        private static AnimationCurve OutQuadToCurve(int samples) => CreateCurve(samples, OutQuad);
        private static AnimationCurve InOutQuadToCurve(int samples) => CreateCurve(samples, InOutQuad);

        // Cubic 函数
        private static float InCubic(float x) => x * x * x;
        private static float OutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);
        private static float InOutCubic(float x) => x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;

        private static AnimationCurve InCubicToCurve(int samples) => CreateCurve(samples, InCubic);
        private static AnimationCurve OutCubicToCurve(int samples) => CreateCurve(samples, OutCubic);
        private static AnimationCurve InOutCubicToCurve(int samples) => CreateCurve(samples, InOutCubic);

        // Quart 函数
        private static float InQuart(float x) => x * x * x * x;
        private static float OutQuart(float x) => 1 - Mathf.Pow(1 - x, 4);
        private static float InOutQuart(float x) => x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;

        private static AnimationCurve InQuartToCurve(int samples) => CreateCurve(samples, InQuart);
        private static AnimationCurve OutQuartToCurve(int samples) => CreateCurve(samples, OutQuart);
        private static AnimationCurve InOutQuartToCurve(int samples) => CreateCurve(samples, InOutQuart);

        // Quint 函数
        private static float InQuint(float x) => x * x * x * x * x;
        private static float OutQuint(float x) => 1 - Mathf.Pow(1 - x, 5);
        private static float InOutQuint(float x) => x < 0.5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;

        private static AnimationCurve InQuintToCurve(int samples) => CreateCurve(samples, InQuint);
        private static AnimationCurve OutQuintToCurve(int samples) => CreateCurve(samples, OutQuint);
        private static AnimationCurve InOutQuintToCurve(int samples) => CreateCurve(samples, InOutQuint);

        // Expo 函数
        private static float InExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
        private static float OutExpo(float x) => x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
        private static float InOutExpo(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;
            return x < 0.5 ? Mathf.Pow(2, 20 * x - 10) / 2 : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
        }

        private static AnimationCurve InExpoToCurve(int samples) => CreateCurve(samples, InExpo);
        private static AnimationCurve OutExpoToCurve(int samples) => CreateCurve(samples, OutExpo);
        private static AnimationCurve InOutExpoToCurve(int samples) => CreateCurve(samples, InOutExpo);

        // Circ 函数
        private static float InCirc(float x) => 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
        private static float OutCirc(float x) => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
        private static float InOutCirc(float x) => x < 0.5
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;

        private static AnimationCurve InCircToCurve(int samples) => CreateCurve(samples, InCirc);
        private static AnimationCurve OutCircToCurve(int samples) => CreateCurve(samples, OutCirc);
        private static AnimationCurve InOutCircToCurve(int samples) => CreateCurve(samples, InOutCirc);

        // Back 函数
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1;

        private static float InBack(float x) => c3 * x * x * x - c1 * x * x;
        private static float OutBack(float x) => 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        private static float InOutBack(float x) => x < 0.5
            ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
            : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;

        private static AnimationCurve InBackToCurve(int samples) => CreateCurve(samples, InBack);
        private static AnimationCurve OutBackToCurve(int samples) => CreateCurve(samples, OutBack);
        private static AnimationCurve InOutBackToCurve(int samples) => CreateCurve(samples, InOutBack);

        // Elastic 函数
        private const float c4 = (2 * Mathf.PI) / 3;
        private const float c5 = (2 * Mathf.PI) / 4.5f;

        private static float InElastic(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;
            return -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4);
        }

        private static float OutElastic(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;
            return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }

        private static float InOutElastic(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;
            if (x < 0.5)
            {
                return -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2;
            }
            else
            {
                return (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
            }
        }

        private static AnimationCurve InElasticToCurve(int samples) => CreateCurve(samples, InElastic);
        private static AnimationCurve OutElasticToCurve(int samples) => CreateCurve(samples, OutElastic);
        private static AnimationCurve InOutElasticToCurve(int samples) => CreateCurve(samples, InOutElastic);

        // Bounce 函数
        private static float OutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                x -= 1.5f / d1;
                return n1 * x * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                x -= 2.25f / d1;
                return n1 * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / d1;
                return n1 * x * x + 0.984375f;
            }
        }

        private static float InBounce(float x) => 1 - OutBounce(1 - x);
        private static float InOutBounce(float x) => x < 0.5
            ? (1 - OutBounce(1 - 2 * x)) / 2
            : (1 + OutBounce(2 * x - 1)) / 2;

        private static AnimationCurve InBounceToCurve(int samples) => CreateCurve(samples, InBounce);
        private static AnimationCurve OutBounceToCurve(int samples) => CreateCurve(samples, OutBounce);
        private static AnimationCurve InOutBounceToCurve(int samples) => CreateCurve(samples, InOutBounce);

        // Flash 函数
        private static float Flash(float x, Ease ease)
        {
            int flashCount = 2;
            float flashProgress = x % (1f / flashCount) * flashCount;

            switch (ease)
            {
                case Ease.InFlash:
                    return InQuad(flashProgress);
                case Ease.OutFlash:
                    return OutQuad(flashProgress);
                case Ease.InOutFlash:
                    return InOutQuad(flashProgress);
                case Ease.Flash:
                default:
                    return flashProgress;
            }
        }

        private static AnimationCurve FlashToCurve(int samples, Ease ease)
        {
            Keyframe[] keys = new Keyframe[samples * 2];

            for (int i = 0; i < samples * 2; i++)
            {
                float t = i / (float)(samples * 2 - 1);
                keys[i] = new Keyframe(t, Flash(t, ease));
            }

            var curve = new AnimationCurve(keys);
            for (int i = 0; i < curve.keys.Length; i++)
                curve.SmoothTangents(i, 0.5f);

            return curve;
        }

        // 通用曲线创建方法
        private static AnimationCurve CreateCurve(int samples, Func<float, float> easingFunction)
        {
            Keyframe[] keys = new Keyframe[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)(samples - 1);
                keys[i] = new Keyframe(t, easingFunction(t));
            }

            var curve = new AnimationCurve(keys);
            for (int i = 0; i < curve.keys.Length; i++)
                curve.SmoothTangents(i, 0.5f);

            return curve;
        }
    }



    #endregion
}