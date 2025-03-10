/****************************************************
    文件：GuidanceVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：Guidance 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class GuidanceVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.Guidance;
        
        
        /// <summary>
        /// 是否是关键引导 
        /// </summary>
        public bool keyGuide;

        /// <summary>
        /// 特定关卡 
        /// </summary>
        public int targetLv;

        /// <summary>
        /// 引导组 
        /// </summary>
        public int guideIndex;

        /// <summary>
        /// 触发引导消息 
        /// </summary>
        public string triggerMsg;

        /// <summary>
        /// 引导描述 
        /// </summary>
        public string desc;

        /// <summary>
        /// 是否有对话 
        /// </summary>
        public bool hasTalk;

        /// <summary>
        /// 对话下标（控制器） 
        /// </summary>
        public int talkIndex;

        /// <summary>
        /// 卡牌位置物体 
        /// </summary>
        public string cardPosObj;

        /// <summary>
        /// 遮罩类型（0无1黑色2圆圈3正方形4透明遮罩5组件提到最高层） 
        /// </summary>
        public int maskBtnType;

        /// <summary>
        /// 遮罩穿透位置 
        /// </summary>
        public float[] maskPos;

        /// <summary>
        /// 遮罩穿透大小 
        /// </summary>
        public float[] maskSize;

        /// <summary>
        /// 完成事件类型（0点击遮罩1消息） 
        /// </summary>
        public int completeType;

        /// <summary>
        /// 最高层组件名 
        /// </summary>
        public string topComName;

        /// <summary>
        /// 完成引导消息 
        /// </summary>
        public string completeMsg;

        /// <summary>
        /// 遮罩穿透物体路径 
        /// </summary>
        public string maskPositionObj;

        /// <summary>
        /// 箭头偏移 
        /// </summary>
        public float[] arrowPostioinOffset;

        /// <summary>
        /// 箭头位置物体 
        /// </summary>
        public string arrowPositionObj;

        /// <summary>
        /// 手位置物体 
        /// </summary>
        public string handPostionObj;

        /// <summary>
        /// 延时自动结束引导 
        /// </summary>
        public float autoFinishTime;

    }
}
        