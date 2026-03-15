/****************************************************
    文件: QuestDispatcher.cs
    作者: Clear
    日期: 2026/3/15 23:45:13
    类型: 逻辑脚本
    功能: Nothing 
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class QuestDispatcher : BaseDispatcher<QuestDispatcher, QuestMsg, QuestEventData> { }
    public class QuestEventData
    {
        public int data;

    }
    public enum QuestMsg
    {
        ClearElement = 101,//清除元素  
        MachElements,//完成一次消除


        QuestAccepted, //接受任务
        QuestCompleted, //任务完成
        QuestProgressUpdated, //任务进度更新


    }
}