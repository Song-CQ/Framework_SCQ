/****************************************************
    文件: Model.cs
    作者: Clear
    日期: 2023/11/23 20:49:21
    类型: MVC_AutoCread
    功能: GameModel
*****************************************************/
using System.Collections.Generic;
using FutureCore;
using ProjectApp.Data;
using UnityEngine;

namespace ProjectApp
{
    public class GameModel : BaseModel
    {

        private QuestModel questModel;

        public Quest currQuest;

        #region 生命周期
        protected override void OnInit()
        {
            questModel = ModuleMgr.Instance.GetModel(ModelConst.QuestModel) as QuestModel;
        }

        protected override void OnDispose()
        {
        }

        protected override void OnReset()
        {
        }
        #endregion

        #region 读取数据
        protected override void OnReadData()
        {

        }

        #endregion

        #region  Task
    

        public void AcceptNextQuest()
        {
            List<Quest> questList = questModel.GetAcceptedQuests();
            if(questList.Count > 0)
            {
                return;
            }

            int index = GameTool.GameCore.TaskIndex % questModel.QuestVOList.Count;
            QuestVO questVO = questModel.QuestVOList[index];
            //注册任务事件
            questModel.AcceptQuest(questVO.QuestID);

            Debug.LogError("注册任务"+questVO.QuestID);


        }

        private void OnQuesrCompleted(QuestEventData data)
        {
            int id = data.questData.questID;
            //领取奖励
            questModel.ClaimReward(id);

            Debug.LogError("领取奖励");
           

            GameTool.GameCore.TaskIndex++;
            //领取下一个任务
            AcceptNextQuest();


        }

        public Quest GetCurrQuest()
        {
            if (questModel.GetAcceptedQuests().Count == 0)
            {
                AcceptNextQuest();
            }
            currQuest = questModel.GetAcceptedQuests()[0];
            return currQuest;
        }



        #endregion




        #region 消息
        protected override void AddListener()
        {
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
            QuestDispatcher.Instance.AddListener(QuestMsg.Completed, OnQuesrCompleted);
            
        }
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
            QuestDispatcher.Instance.RemoveListener(QuestMsg.Completed, OnQuesrCompleted);
        }
        #endregion

    }
}