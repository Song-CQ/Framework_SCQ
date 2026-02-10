using System;


namespace FutureCore
{
    public class JudgeData : IObjectPoolItem
    {
        //总经过帧数
        public int runFrame;
        //当前阶段索引
        public int step;
        //包含的comboData
        public ComboData comboData;
        //HD的成功帧计数
        public float count = 0;
        //计数准备，将在计数一次后置false，每帧开始时重置为true
        public bool countReady;

        public JudgeData()
        {
        }

        public void Dispose()
        {
            comboData = null;
        }

        public void Reset()
        {
            runFrame = 0;
            step = 0;
            comboData = null;
            count = 0;
            countReady = false;
        }
        public void FillData(JudgeData data)
        {
            runFrame = data.runFrame;
            step = data.step;
            comboData = data.comboData;
            count = data.count;
            countReady = data.countReady;
        }
    }
}

