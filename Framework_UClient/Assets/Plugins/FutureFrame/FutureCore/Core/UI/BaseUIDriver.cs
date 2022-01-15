/****************************************************
    文件：BaseUIDriver.cs
	作者：Clear
    日期：2022/1/15 18:6:37
    类型: 框架核心脚本(请勿修改)
	功能：基础UI驱动
*****************************************************/
namespace FutureCore
{
    public abstract class BaseUIDriver
    {      
        public abstract void Register();
        public abstract void Init();
        public abstract void Unregister();
    }
}