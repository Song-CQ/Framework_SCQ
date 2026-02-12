/****************************************************
    文件: PlayerData.cs
    作者: Clear
    日期: 2026/2/12 1:11:7
    类型: 逻辑脚本
    功能: 玩家数据
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class ExternalProp_PlayerData : IPlayerData
    {
        public Dictionary<ExternalProp, int> allExternalProp = new Dictionary<ExternalProp, int>();

        public ExternalProp_PlayerData()
        {
            allExternalProp.Add( ExternalProp.Hammer,10); 
            allExternalProp.Add( ExternalProp.AllRandom,10); 
            allExternalProp.Add( ExternalProp.Horizontal,10); 
            allExternalProp.Add( ExternalProp.Vertical,10); 
            allExternalProp.Add( ExternalProp.Swipe,10); 
            allExternalProp.Add( ExternalProp.Undo,10); 
            allExternalProp.Add( ExternalProp.Wild,10); 
            allExternalProp.Add( ExternalProp.AddScore,10); 
            
        }
    }
}