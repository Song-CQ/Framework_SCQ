/****************************************************
    文件: QuestManager.cs
    作者: Clear
    日期: 2026/3/15 14:37:39
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using System.Collections.Generic;
using System;
using UnityEngine;
using FutureCore;
using ProjectApp.Data;
using System.Collections;

namespace ProjectApp
{


    // 任务管理器
    public class QuestManager : BaseMgr<QuestManager>
    {
        private Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        private Dictionary<QuestStatus, List<Quest>> questsByStatus = new Dictionary<QuestStatus, List<Quest>>();

        // 事件
        public event System.Action<Quest> OnQuestAccepted;
        public event System.Action<Quest> OnQuestCompleted;
        public event System.Action<Quest> OnQuestProgressUpdated;
        public override void StartUp()
        {
            base.StartUp();
            AppDispatcher.Instance.AddListener(AppMsg.System_ConfigInitComplete,ConfigInit);


        }

        private void ConfigInit(object obj)
        {
            Initialize();
        }

        private void Initialize()
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

                allQuests.Add(q.questID,q);

            }

        }

        private Quest CreateQuest(QuestVO item)
        {
            Quest quest = new Quest();

            quest.questID = item.QuestID;
            quest.description = item.Description;
            quest.questType = (QuestType)item.QuestType;
            string[] data = item.QuestGoals;
            quest.goals.Add(CreadQuestGoal(data));
            return quest;
            




        }


        private QuestGoal CreadQuestGoal(string[] datas)
        {
            Enum.TryParse(datas[0],true,out QuestMsg key);

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

            LogUtil.LogError("转换条件失败！"+key);
            return null;


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

            // 触发事件
            OnQuestAccepted?.Invoke(quest);

            // 保存进度
            SaveQuestProgress();

            return true;
        }

        // 完成任务
        public bool CompleteQuest(int questID)
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

            // 触发事件
            OnQuestCompleted?.Invoke(quest);

            // 保存进度
            SaveQuestProgress();

            return true;
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
            if (QuestManager.Instance.AcceptQuest(questID))
            {
                Debug.Log("任务接取成功！");
            }
            else
            {
                Debug.Log("无法接取任务");
            }
        }
    }
}