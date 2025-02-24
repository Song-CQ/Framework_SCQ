/****************************************************
    文件: BaseRoleEntity.cs
    作者: Clear
    日期: 2023/12/6 11:29:54
    类型: 逻辑脚本
    功能: 基础角色
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    public class BaseRole: IRole,IFill
    {
        public IFill FillModule => this;
        public ISceneEvent sceneEvent => throw new System.NotImplementedException();

        public string Key { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


        public void EnterPicture(IPicture picture)
        {
            throw new System.NotImplementedException();
        }

        public void ExitPicture(IPicture picture)
        {
            throw new System.NotImplementedException();
        }
    }
}