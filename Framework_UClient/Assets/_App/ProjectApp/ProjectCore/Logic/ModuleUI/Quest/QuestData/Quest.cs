/****************************************************
    文件: Quest.cs
    作者: Clear
    日期: 2026/3/15 14:41:22
    类型: 逻辑脚本
    功能: 任务类
*****************************************************/
using System.Collections.Generic;
using System;
using UnityEngine;
using FutureCore;

namespace ProjectApp
{
    // 任务状态枚举
    public enum QuestStatus
    {
        Locked,     // 未解锁
        Available,  // 可接取
        Accepted,   // 进行中
        Completed,  // 已完成
        Failed,     // 失败
        Rewarded    // 已领奖
    }

    // 任务类型
    public enum QuestType
    {
        Daily = 0,       // 日常
        Main,      // 主线 
        Side,       // 支线
        Achievement // 成就
    }

    // 基础任务类
    [System.Serializable]
    public class Quest
    {
        public int questID;
        public string questName;
        public string description;
        public QuestType questType;
        public QuestStatus status;

        // 任务目标
        public List<QuestGoal> goals;

        // 任务奖励
        public List<QuestReward> rewards;

        // 前置任务
        public List<int> prerequisiteQuests;

        // 任务数据
        public bool isNew;
        public DateTime acceptTime;
        public DateTime completeTime;
    }

    
}