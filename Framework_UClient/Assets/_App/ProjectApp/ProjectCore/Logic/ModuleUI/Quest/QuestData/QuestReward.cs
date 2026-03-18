/****************************************************
    文件: QuestReward.cs
    作者: Clear
    日期: 2026/3/15 19:45:11
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using System;
using UnityEngine;

namespace ProjectApp
{
    public class QuestReward
    {
        public ExternalProp externalProp;
        public uint sum;
        public void Grant()
        {
           ExternalProp_PlayerData  externalProp_PlayerData = PlayerDataMgr.Instance.GetData<ExternalProp_PlayerData>();
           externalProp_PlayerData.SetExternalPropSum(externalProp,sum);

        }
    }
}