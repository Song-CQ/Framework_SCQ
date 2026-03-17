/****************************************************
    文件: Model.cs
    作者: Clear
    日期: 2023/11/23 20:49:21
    类型: MVC_AutoCread
    功能: GameModel
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class GameModel:BaseModel
    {

        private QuestModel questModel;

        private Quest currQuest;

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


        public void GetCurrQuest()
        {
            currQuest = questModel.GetAcceptedQuests()[0];
        }
      
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

    }
}