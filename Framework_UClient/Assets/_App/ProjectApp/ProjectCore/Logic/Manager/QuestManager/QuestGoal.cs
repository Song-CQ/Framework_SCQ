/****************************************************
    文件: QuestGoal.cs
    作者: Clear
    日期: 2026/3/15 19:46:23
    类型: 逻辑脚本
    功能: Nothing 
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{

    
    // 任务目标基类
    [System.Serializable]
    public abstract class QuestGoal
    {
        public string description;
        public int currentAmount;
        public int targetAmount;
        public bool isCompleted;

        public abstract void Init();
        public abstract void UpdateProgress();
        public abstract string GetProgressText();
    }

    // 收集目标
    [System.Serializable]
    public class ClearElement_Goal : QuestGoal
    {
        public const QuestMsg Key = QuestMsg.ClearElement;
        public ElementType itemType;

        public override void Init()
        {
            QuestDispatcher.Instance.AddListener(QuestMsg.ClearElement, OnItemCollected);
        }

        private void OnItemCollected(QuestEventData evt)
        {
            //if (evt.itemID == itemID)
            //{
            //    currentAmount = Mathf.Min(currentAmount + evt.amount, targetAmount);
            //    isCompleted = currentAmount >= targetAmount;
            //    EventManager.Instance.TriggerEvent(new QuestProgressUpdateEvent());
            //}
        }

        public override string GetProgressText()
        {
            return $" {currentAmount}/{targetAmount}";
        }

        public override void UpdateProgress()
        {
            throw new System.NotImplementedException();
        }
    }

    // 完成几消
    [System.Serializable]
    public class MatchElements_Goal : QuestGoal
    {
        public const QuestMsg Key = QuestMsg.MachElements;

        public int Sum;


        public override void Init()
        {
            QuestDispatcher.Instance.AddListener(QuestMsg.MachElements, OnMatchElements);
        }

        private void OnMatchElements(QuestEventData evt)
        {
            //if (evt.enemyID == enemyID)
            //{
            //    currentAmount = Mathf.Min(currentAmount + 1, targetAmount);
            //    isCompleted = currentAmount >= targetAmount;
            //    EventManager.Instance.TriggerEvent(new QuestProgressUpdateEvent());
            //}
        }

        public override string GetProgressText()
        {
            return $"击杀 {currentAmount}/{targetAmount}";
        }

        public override void UpdateProgress()
        {
            throw new System.NotImplementedException();
        }
    }

  
}