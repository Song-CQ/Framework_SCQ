/****************************************************
    文件: GameTool.cs
    作者: Clear
    日期: 2024/1/16 12:8:52
    类型: 逻辑脚本
    功能: 游戏逻辑工具
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public static class GameLogicConst
    {
        public const string None = "None";


    }

    public static class GameLogicTool
    {
         

        public static ComboEventData FillComboData(ComboEventData combo, BasePicture basePicture)
        {
            combo.IsComplete = false;
            combo.SceneEvent_Key = GameLogicConst.None;
            combo.RoleEvent_Keys.Clear();
            if (basePicture.SceneEvent==null|| basePicture.SceneEvent.FillModule.Key==string.Empty)
            {
                return combo;
            }

            combo.SceneEvent_Key = basePicture.SceneEvent.FillModule.Key;

            foreach (var item in basePicture.Roles)
            {
                if (item == null||item.FillModule.Key==string.Empty)
                {
                    combo.RoleEvent_Keys.Add(GameLogicConst.None);
                }
                else
                {
                    combo.RoleEvent_Keys.Add(item.FillModule.Key);
                }
            }
            combo.IsComplete = basePicture.CheckComplete();
            return combo;
        }

        /// <summary>
        /// 检测该角色是否可以与 该事件产生combo
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="sceneEvent"></param>
        /// <returns></returns>
        public static bool CanRoleSetSceneEvent(IRole role, BaseSceneEvent sceneEvent)
        {
            return GameWorldMgr.Instance.CanRoleSetSceneEvent(role, sceneEvent); 
        }

        /// <summary>
        /// 获取是否有对应的场景
        /// </summary>
        /// <param name="key"></param>
        /// <param name="needSceneEvent"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetHaveSceneKeyToIndex(string SceneEventKey, int index = -1)
        {

            return GameWorldMgr.Instance.GetHaveSceneKeyToIndex(SceneEventKey, index);


        }
    }
} 