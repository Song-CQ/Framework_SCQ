/****************************************************
    文件:Model.cs
    作者:Clear
    日期:2022/1/29 23:8:15
    类型:MVC_AutoCread
    功能:MainModel
*****************************************************/
using FutureCore;

namespace ProjectApp.HotFix
{
    public class MainModel:BaseModel
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
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
        }
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
        }
        #endregion

    }
}