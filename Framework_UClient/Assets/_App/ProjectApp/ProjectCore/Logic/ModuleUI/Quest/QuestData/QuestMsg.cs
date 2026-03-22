/****************************************************
    文件: QuestDispatcher.cs
    作者: Clear
    日期: 2026/3/15 23:45:13
    类型: 逻辑脚本
    功能: Nothing 
*****************************************************/
using System;
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public enum QuestMsg
    {
        ClearElement = 101,//清除元素  
        MachElements,//完成一次消除


        Accepted, //接受任务
        Completed, //任务完成
        ProgressUpdated, //任务进度更新


    }
}