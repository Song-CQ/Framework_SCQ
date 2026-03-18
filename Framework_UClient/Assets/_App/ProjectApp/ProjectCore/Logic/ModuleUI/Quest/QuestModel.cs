/****************************************************
    文件: Model.cs
    作者: Clear
    日期: 2026/3/16 19:6:40
    类型: MVC_AutoCread
    功能: QuestModel
*****************************************************/
using System;
using System.Collections.Generic;
using FutureCore;
using ProjectApp.Data;
using UnityEngine;

namespace ProjectApp
{
    public class QuestModel : BaseModel
    {
        private Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        private Dictionary<QuestStatus, List<Quest>> questsByStatus = new Dictionary<QuestStatus, List<Quest>>();

        public List<QuestVO> QuestVOList;

        

        #region 生命周期

        protected override void OnInit()
        {
            QuestGoal.questModel = this;

        }

        #region api

        /// <summary>
        /// 更新任务进度
        /// </summary>
        /// <param name="questID"></param>
        public void ProgressUpdated(int questID)
        {
            if (!allQuests.ContainsKey(questID)) return;

            Quest quest = allQuests[questID];

            QuestEventData eventData = EventData.GetEvent<QuestEventData>();
            eventData.questData = quest;
            QuestDispatcher.Instance.Dispatch(QuestMsg.ProgressUpdated, eventData);



            // 检查是否所有目标都完成
            CompleteQuest(questID);



        }

        // 接取任务 
        public bool AcceptQuest(int questID)
        {
            if (!allQuests.ContainsKey(questID)) return false;

            Quest quest = allQuests[questID];

            // 检查前置任务
            if (!CheckPrerequisites(quest)) return false;

            // 检查任务状态
            if (quest.status != QuestStatus.Available) return false;

            // 更新状态
            quest.status = QuestStatus.Accepted;
            quest.acceptTime = DateTime.Now;
            quest.isNew = true;

            // 初始化任务目标
            foreach (var goal in quest.goals)
            {
                goal.Init();
            }

            // 分类存储
            questsByStatus[QuestStatus.Accepted].Add(quest);
            questsByStatus[QuestStatus.Available].Remove(quest);

            QuestEventData eventData = EventData.GetEvent<QuestEventData>();
            eventData.questData = quest;
            // 触发事件
            QuestDispatcher.Instance.Dispatch(QuestMsg.Accepted, eventData);

            // 保存进度
            SaveQuestProgress();

            return true;
        }

        // 完成任务
        private bool CompleteQuest(int questID)
        {
            if (!allQuests.ContainsKey(questID)) return false;

            Quest quest = allQuests[questID];

            // 检查是否所有目标都完成
            if (!CheckAllGoalsCompleted(quest)) return false;

            // 更新状态
            quest.status = QuestStatus.Completed;
            quest.completeTime = DateTime.Now;

            // 分类更新
            questsByStatus[QuestStatus.Accepted].Remove(quest);
            questsByStatus[QuestStatus.Completed].Add(quest);

            // 解锁后续任务
            UnlockNextQuests(quest);

            QuestEventData eventData = EventData.GetEvent<QuestEventData>();
            eventData.questData = quest;
            // 触发事件
            QuestDispatcher.Instance.Dispatch(QuestMsg.Completed, eventData);

            // 保存进度
            SaveQuestProgress();

            return true;
        }

        // 领取奖励


        #endregion



        protected override void OnReadData()
        {
            QuestVOList = QuestVOModel.Instance.GetVOList();

            InitQuset();
        }

        protected override void OnDispose()
        {
            QuestGoal.questModel = null;
            allQuests = null;
            foreach (var item in questsByStatus)
            {
                item.Value.Clear();
            }
            questsByStatus = null;
        }


        protected override void OnReset()
        {
        }
        #endregion

        #region 消息
        protected override void AddListener()
        {
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
        }
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
        }
        #endregion


        private void InitQuset()
        {
            // 初始化任务分类字典
            foreach (QuestStatus status in System.Enum.GetValues(typeof(QuestStatus)))
            {
                questsByStatus[status] = new List<Quest>();
            }



            // 加载任务数据
            LoadQuests();

            // 加载存档
            LoadQuestProgress();
        }

        private void LoadQuests()
        {
            foreach (var item in QuestVOModel.Instance.GetVOList())
            {
                Quest q = CreateQuest(item);

                allQuests.Add(q.questID, q);

            }


        }

        private Quest CreateQuest(QuestVO item)
        {
            Quest quest = new Quest();

            quest.questID = item.QuestID;
            quest.description = item.Description;
            quest.questType = (QuestType)item.QuestType;
            quest.goals = new List<QuestGoal>();

            //默认解锁
            quest.status = QuestStatus.Available;

            string[] data = item.QuestGoals;
            QuestGoal questGoal = CreadQuestGoal(data);
            questGoal.questData = quest;
            quest.goals.Add(questGoal);

            return quest;

        }


        private QuestGoal CreadQuestGoal(string[] datas)
        {
            Enum.TryParse(datas[0], true, out QuestMsg key);

            switch (key)
            {
                case ClearElement_Goal.Key://消除元素任务
                    {
                        var goal = new ClearElement_Goal();
                        if (Enum.TryParse(datas[1], true, out ElementType type))  // true=忽略大小写
                        {
                            goal.itemType = type;
                        }
                        goal.targetAmount = int.Parse(datas[2]);

                        return goal;
                    }

                case MatchElements_Goal.Key:
                    {
                        var goal = new MatchElements_Goal();
                        goal.Sum = int.Parse(datas[1]);
                        goal.targetAmount = int.Parse(datas[2]);
                        return goal;
                    }
            }

            LogUtil.LogError("转换条件失败！" + key);
            return null;


        }


        // 领取奖励
        public bool ClaimReward(int questID)
        {
            if (!allQuests.ContainsKey(questID)) return false;

            Quest quest = allQuests[questID];
            if (quest.status != QuestStatus.Completed) return false;

            // 发放奖励
            foreach (var reward in quest.rewards)
            {
                reward.Grant();
            }

            quest.status = QuestStatus.Rewarded;
            questsByStatus[QuestStatus.Rewarded].Add(quest);
            questsByStatus[QuestStatus.Completed].Remove(quest);

            SaveQuestProgress();
            return true;
        }

        // 获取可接任务列表
        public List<Quest> GetAvailableQuests()
        {
            return questsByStatus[QuestStatus.Available];
        }

        // 获取进行中任务
        public List<Quest> GetAcceptedQuests()
        {
            return questsByStatus[QuestStatus.Accepted];
        }

        // 检查前置任务
        private bool CheckPrerequisites(Quest quest)
        {
            if(quest.prerequisiteQuests !=null)
            foreach (int prereqID in quest.prerequisiteQuests)
            {
                if (!allQuests.ContainsKey(prereqID)) continue;

                Quest prereqQuest = allQuests[prereqID];
                if (prereqQuest.status != QuestStatus.Completed &&
                    prereqQuest.status != QuestStatus.Rewarded)
                {
                    return false;
                }
            }
            return true;
        }

        // 检查所有目标
        private bool CheckAllGoalsCompleted(Quest quest)
        {
            foreach (var goal in quest.goals)
            {
                if (!goal.isCompleted) return false;
            }
            return true;
        }

        // 解锁后续任务
        private void UnlockNextQuests(Quest completedQuest)
        {
            foreach (var quest in allQuests.Values)
            {
                if (quest.prerequisiteQuests.Contains(completedQuest.questID) &&
                    quest.status == QuestStatus.Locked &&
                    CheckPrerequisites(quest))
                {
                    quest.status = QuestStatus.Available;
                    questsByStatus[QuestStatus.Available].Add(quest);
                    questsByStatus[QuestStatus.Locked].Remove(quest);
                }
            }
        }

        // 保存/加载
        private void SaveQuestProgress()
        {
            QuestSaveData saveData = new QuestSaveData();
            foreach (var quest in allQuests.Values)
            {
                saveData.questStatuses.Add(quest.questID, (int)quest.status);
                // 保存目标进度
                foreach (var goal in quest.goals)
                {
                    //saveData.goalProgresses.Add(goal.description, goal.currentAmount);
                }
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString("QuestProgress", json);
            PlayerPrefs.Save();
        }

        private void LoadQuestProgress()
        {
            if (PlayerPrefs.HasKey("QuestProgress"))
            {
                string json = PlayerPrefs.GetString("QuestProgress");
                QuestSaveData saveData = JsonUtility.FromJson<QuestSaveData>(json);

                // 恢复任务状态和进度
                // ...
            }
            else
            {
                // 初始化新手任务
                //InitializeNewGameQuests();
            }
        }

    }

    public class QuestEventData : EventData
    {
        public const string KEY = "QuestEventData";
        public Quest questData;
        public override void Reset()
        {
            base.Reset();
            questData = null;
        }
    }
    public class QuestDispatcher : BaseDispatcher<QuestDispatcher, QuestMsg, QuestEventData>
    {
        protected new bool IsAutoReturnEventData = true;

    }



    [System.Serializable]
    public class QuestSaveData
    {
        public Dictionary<int, int> questStatuses = new Dictionary<int, int>();
        public Dictionary<int, int> goalProgresses = new Dictionary<int, int>();
    }

    // 使用示例
    public class QuestGiver : MonoBehaviour
    {
        public int questID;

        public void OnInteract()
        {

        }
    }
}