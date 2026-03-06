/****************************************************
    文件: PlayerDataMgr.cs
    作者: Clear
    日期: 2026/2/12 0:58:9
    类型: 逻辑脚本
    功能: 玩家数据
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{

    public interface IPlayerData
    {
        
    }

    public class PlayerDataMgr : BaseMgr<PlayerDataMgr>
    {
        private Dictionary<Type, IPlayerData> allPlayerData = new Dictionary<Type, IPlayerData>();


        public override void Init()
        {
            base.Init();

            // 使用示例
            RegisterData(new ExternalProp_PlayerData());
            //RegisterData(new PlayerInventoryData());

        }

        // 注册所有数据实例
        public void RegisterData<T>(T data) where T : IPlayerData
        {
            Type type = typeof(T);
            if (!allPlayerData.ContainsKey(type))
            {
                allPlayerData[type] = data;
            }
        }

        public T GetData<T>() where T : IPlayerData
        {
            Type type = typeof(T);

            if (allPlayerData.TryGetValue(type, out IPlayerData data))
            {
                return (T)data;
            }

            Debug.LogError($"数据 {typeof(T)} 未注册！");
            return default;
        }
    }

   
}