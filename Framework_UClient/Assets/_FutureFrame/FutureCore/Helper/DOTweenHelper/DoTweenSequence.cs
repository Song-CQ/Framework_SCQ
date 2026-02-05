using DG.Tweening;

namespace FutureCore
{
    public abstract class DoTweenSequence
    {
        protected Sequence sequence;

        public bool IsPlay;
        public bool IsPause;
        public bool IsComplete;

        

        public DoTweenSequence()
        {
            CreadSequence();
        }

        private void CreadSequence()
        {
            // 创建一个动画序列
            sequence = DOTween.Sequence();
            //sequence.SetAutoKill(false).SetRecyclable(true); 
            sequence.SetAutoKill(false);




            AddTweenToSequence(sequence);
            // ========== 第一阶段：顺序动画 ==========

            // 1. 先移动到位置A


            //// 2. 然后旋转90度
            //sequence.Append(target.DORotate(new Vector3(0, 90, 0), duration));

            //// ========== 第二阶段：同时播放的动画 ==========

            //// 3. 移动到位置B的同时改变颜色（假设有Renderer）
            //sequence.Append(target.DOMove(new Vector3(2, 2, 0), duration));
            ////sequence.Join(GetComponent<Renderer>().material.DOColor(Color.red, duration));

            //// 4. 然后缩放和旋转同时进行
            //sequence.Append(target.DOScale(Vector3.one * 2, duration));
            //sequence.Join(target.DORotate(new Vector3(0, 180, 0), duration));

            // 动画完成时回调
            sequence.OnComplete(OnComplete);

            // 动画开始时回调
            sequence.OnStart(OnStart);
            // 设置缓动函数
            sequence.SetEase(Ease.Linear);
            sequence.Pause();

        }

        protected abstract void AddTweenToSequence(Sequence seq);

        public void Play()
        {
            // 播放动画
            //sequence.Rewind();
            //sequence.Play();
            sequence.Restart();
            IsPlay = true;
        }
        public void Pause()
        {
            IsPause = true;
            sequence.Pause();
        }

        public void CanlePause()
        {
            IsPause = false;
            sequence.Play();
        }


        public void Stop()
        {
            sequence.Complete();//调用OnComplete
            sequence.Pause();
            //sequence.Rewind(); 将动画回到第一针 会导致动画影响的物体回拉 一般在会开始的时候用
            IsPlay = false;
            ResetState();
        }


        protected virtual void OnStart()
        {
           

        }
        protected virtual void OnComplete()
        {
            IsPlay = false;
            IsComplete = true;
        }

        public virtual void ResetState()
        {
            IsComplete = false;
            IsPause = false;
            IsPlay = false;

        }

        public virtual void Disp()
        {
            ResetState();
            sequence.Kill();
            sequence = null;
            

        }


    }
    

}
