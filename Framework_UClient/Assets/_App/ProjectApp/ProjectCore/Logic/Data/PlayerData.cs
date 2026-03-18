/****************************************************
    文件: PlayerData.cs
    作者: Clear
    日期: 2026/2/12 1:11:7
    类型: 逻辑脚本
    功能: 玩家数据
*****************************************************/
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ProjectApp
{
    public interface IPlayerData
    {
        void SaveData();
    }

    public class BasePlayerData:IPlayerData
    {

        public virtual void SaveData()
        {
            PlayerDataDispatcher.Instance.Dispatch(PlayerDataMsg.Updata,this);
        }


    }

    public class ExternalProp_PlayerData : BasePlayerData
    {

        public Dictionary<ExternalProp, uint> allExternalProp = new Dictionary<ExternalProp, uint>();

        public ExternalProp_PlayerData()
        {
            allExternalProp.Add(ExternalProp.Hammer, 10);
            allExternalProp.Add(ExternalProp.AllRandom, 10);
            allExternalProp.Add(ExternalProp.Horizontal, 10);
            allExternalProp.Add(ExternalProp.Vertical, 10);
            allExternalProp.Add(ExternalProp.Swipe, 10);
            allExternalProp.Add(ExternalProp.Undo, 10);
            allExternalProp.Add(ExternalProp.Wild, 10);
            allExternalProp.Add(ExternalProp.AddScore, 10);

        }

        public void SetExternalPropSum(ExternalProp externalProp,uint newVal)
        {
            allExternalProp[externalProp] = newVal;
            SaveData();
        }

        public uint GetExternalPropSum(ExternalProp externalProp)
        {
            if(allExternalProp.TryGetValue(externalProp,out uint val))
            {
                return val;
            }
            return 0;
        }

        
    }
}