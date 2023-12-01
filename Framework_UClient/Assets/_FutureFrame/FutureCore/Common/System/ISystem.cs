/****************************************************
    文件: ISystem.cs
    作者: Clear
    日期: 2023/12/1 15:56:13
    类型: 逻辑脚本
    功能: 基础系统接口
*****************************************************/
namespace FutureCore
{
    public interface ISystem 
    {
        public bool IsInit { get; }
        public bool IsRunning { get; }

        public void Init();

        public void Start();
        public void Shutdown();

        public void Display();
    }
}