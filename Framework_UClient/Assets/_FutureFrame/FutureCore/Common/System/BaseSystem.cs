/****************************************************
    文件: BaseSystem.cs
    作者: Clear
    日期: 2023/12/1 15:46:37
    类型: 框架核心脚本(请勿修改)
    功能: 基础系统
*****************************************************/
namespace FutureCore
{
    public class BaseSystem : ISystem
    {
        private bool isInit = false;
        private bool isRunning = false;
        public bool IsInit => isInit;

        public bool IsRunning => isRunning;

        public virtual void Init()
        {
            isInit = true;
        }    

        public virtual void Start()
        {
            isRunning = true;
        }

        public virtual void Shutdown()
        {
            isRunning = false;
        }

        public virtual void Display()
        {
            isInit = false;
            isRunning = false;
        }
    }
}