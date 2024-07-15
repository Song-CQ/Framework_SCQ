/****************************************************
    文件: interface.cs
    作者: Clear
    日期: 2023/12/6 11:22:7
    类型: 逻辑脚本
    功能: 接口
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public interface IPicture//画面
    {
        public ISceneEvent SceneEvent { get;}
        public List<IRole> Roles { get; }
        public ComboEventData Combo { get;}
        public int Index { get; }



        public Action<ComboEventData> ChangeCombo_Event { get; }//变化事件
        public void Init();
        public void SetScene(ISceneEvent newScene);

        public bool AddRole(IRole role, int index = -1);
        public bool RemoveRole(int index);

        public void Rest();


    }

    public interface ISceneEvent//事件
    {
        /// <summary>
        /// 填充模块
        /// </summary>
        public IFill FillModule { get;}
        public List<IRole> Roles { get; }

        /// <summary>
        /// 事件是否 被完成完成  完成应该要触发一个完成事件
        /// </summary>
        public bool IsComplete { get; }

    }

    
    public interface IRole//角色
    {
        /// <summary>
        /// 填充模块
        /// </summary>
        public IFill FillModule { get; }
        public ISceneEvent sceneEvent { get; }

    }

    public interface IFill
    {

        public string Key { set; get; }

        public void EnterPicture(IPicture picture);
        public void ExitPicture(IPicture picture);
    }
}
/*
 *   IPicture 被修改 发送ChaneComboEvent
 * 
 *   是否胜利 判断是否有该局固定的  ComboEvent 被成功触发
 * 
 * 
 */