/****************************************************
    文件: RoleData.cs
    作者: Clear
    日期: 2024/8/13 17:5:21
    类型: 逻辑脚本
    功能: 人物数据
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class Role_Data: Base_Data
    {
        public override string Desc => key.ToString();

        public RoleKey key;

        public List<LabelKey> allLabelKey;

     
    }
}