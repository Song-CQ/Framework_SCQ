using System;
using UnityEngine;
namespace FutureCore
{
    /// <summary>
    /// 判定规则数据
    /// </summary>
    public class InputOrderData
    {
        // 键控代码
        public string code;
        // 键标签,有 "up","down","left","right","a","b","c","d"等
        public string label;
        // 键输入判定类型,有: KD KU KH KW 等
        public int type;
        // 判定生效下限
        public int lowLim;
        // 判定生效上限
        public int highLim;
        // 持续帧数，在KH和KW时做比对
        public int duration;

        public InputOrderData(string code, string label, int type, int low, int high, int duration)
        {
            this.code = code;
            this.label = label;
            this.type = type;
            this.lowLim = low;
            this.highLim = high;
            this.duration = duration;
        }

        public InputOrderData(string code, string label, int type, int low, int high)
        {
            this.code = code;
            this.label = label;
            this.type = type;
            this.lowLim = low;
            this.highLim = high;
            this.duration = 0;
        }

        public string toString()
        {
            return "code:" + code + " label:" + label + " type:" + type + " lowLim:" + lowLim + " highLow:" + highLim;
        }
    }
}

