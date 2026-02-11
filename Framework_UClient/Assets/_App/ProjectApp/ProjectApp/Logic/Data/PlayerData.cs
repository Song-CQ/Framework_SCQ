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
    }
}