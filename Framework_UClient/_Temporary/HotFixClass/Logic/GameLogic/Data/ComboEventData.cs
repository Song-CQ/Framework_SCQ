/****************************************************
    文件: NewScript.cs
    作者: Clear
    日期: 2024/1/16 12:18:6
    类型: 逻辑脚本
    功能: 组合事件
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
        /*
         *    事件  角色  角色
         *    配置：event|role1,role2
         *    
         *    每个画面都有个Combo 也有自己关注的combo 当自身处于可响应combo状态 就根据是否有自己关注的combo去显示一个自身是否是已经触发状态
         *    
         *    
         */
    /// <summary>        
    /// 组合事件： 
    /// </summary>
    public class ComboEventData
    {

        public ComboEventData(int _index)
        {
            index = _index;
        }

        

        /// <summary>
        /// 触发Combo的步数（画面index）
        /// </summary>
        public int index;
        /// <summary>
        /// 触发的场景事件
        /// </summary>
        public string SceneEvent_Key;
        /// <summary>
        /// 触发的角色
        /// </summary>
        public List<string> RoleEvent_Keys;

        /// <summary>
        /// 是否 已经完成
        /// </summary>
        public bool IsComplete;
    }
}