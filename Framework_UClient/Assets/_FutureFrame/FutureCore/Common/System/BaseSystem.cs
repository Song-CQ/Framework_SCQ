/****************************************************
    文件: BaseSystem.cs
    作者: Clear
    日期: 2023/12/1 15:46:37
    类型: 框架核心脚本(请勿修改)
    功能: 基础系统
*****************************************************/
using UnityEditor;

namespace FutureCore
{
    public class BaseSystem : ISystem
    {
        private bool isInit = false;
        private bool isRunning = false;
        public bool IsInit => isInit;

        public bool IsRunning => isRunning;
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            isInit = true;
        }
        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Start()
        {
            isRunning = true;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Shutdown()
        {
            isRunning = false;
        }
        /// <summary>
        /// 运行
        /// </summary>
        public virtual void Run()
        {
        
        }
		/// <summary>
        /// 将Run方法注册到Update
        /// </summary>
        public void AddRun_To_UpdataFrame()
        {
            TimerMgr.UpData_Event_ToFrame += Run;
        }
        
		/// <summary>
        /// 将Run方法取消注册Update
        /// </summary>
        public void RemoveRun_To_UpdataFrame()
        {
            TimerMgr.UpData_Event_ToFrame -= Run;
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Dispose()
        {
            isInit = false;
            isRunning = false;
        }
    }
}