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
        public static QuestModel questModel;
        public Quest questData;

        public string description;
        public int currentAmount;
        public int targetAmount;
        public bool isCompleted;

        public abstract void Init();
        public virtual void UpdateProgress()
        {
            questModel.ProgressUpdated(questData.questID);
            if (currentAmount >= targetAmount)
            {
                isCompleted = true;
            }
        }

        public virtual string GetProgressText()
        {
            return $"{description} {currentAmount}/{targetAmount}";
        }
    }

    // 收集目标
    [System.Serializable]
    public class ClearElement_Goal : QuestGoal
    {
        public const QuestMsg Key = QuestMsg.ClearElement;
        public ElementType itemType;

        public override void Init()
        {
            QuestDispatcher.Instance.AddListener(QuestMsg.ClearElement, OnClearElement);

        }

        private void OnClearElement(QuestEventData evt)
        {

            ElementType tarType = (ElementType)evt.dataInt;
            if (tarType == itemType)
            {
                currentAmount += 1;
                UpdateProgress();
            }

         
        }

        public override string GetProgressText()
        {
            return $" {currentAmount}/{targetAmount}";
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
            if(evt.dataInt>Sum)
            {
                currentAmount += 1;
                UpdateProgress();
            }


          
        }




    }


}