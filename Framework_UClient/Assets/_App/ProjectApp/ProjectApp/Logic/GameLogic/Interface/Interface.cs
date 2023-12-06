/****************************************************
    文件: interface.cs
    作者: Clear
    日期: 2023/12/6 11:22:7
    类型: 逻辑脚本
    功能: 接口
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public interface IPicture
    {
        public IEvent Scene { get;}

        public void Init();
        public void SetScene(IEvent newScene);
        public void Show(int index);
        public void Rest();
    }

    public interface IEvent
    {
        public List<IRole> Roles { get; }
        public void Exit();
        public void Show();

        public bool AddRole(IRole role, int index = -1);
        public bool RemoveRole(int index);
    }


    public interface IRole
    {

    }
}