/****************************************************
    文件: Model.cs
    作者: Clear
    日期: 2022/5/3 14:23:13
    类型: MVC_AutoCread
    功能: LoadingModel
*****************************************************/
using UnityEngine;
using FutureCore;

namespace ProjectApp
{
    public class LoadingModel:BaseModel
    {

        #region 生命周期
        protected override void OnInit()
        {
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
      
        #region 消息
        protected override void AddListener()
        {
            //AppDispatcher.Instance.AddPriorityListener(AppMsg.UI_DisplayLoadingUI, OpenUI);
            //AppDispatcher.Instance.AddPriorityListener(AppMsg.UI_HideLoadingUI, CloseUI);
        }
        protected override void RemoveListener()
        {
            //AppDispatcher.Instance.RemovePriorityListener(AppMsg.UI_DisplayLoadingUI, OpenUI);
            //AppDispatcher.Instance.RemovePriorityListener(AppMsg.UI_HideLoadingUI, CloseUI);
        }
        #endregion

    }
}