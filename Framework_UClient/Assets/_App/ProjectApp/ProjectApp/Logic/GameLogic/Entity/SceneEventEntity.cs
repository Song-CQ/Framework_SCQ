/****************************************************
    文件: SceneEventEntity.cs
    作者: Clear
    日期: 2024/8/15 18:2:31
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class SceneEventEntity :BaseSceneEvent ,IDrag
    {

        public List<Transform> RoleSoltPot = new List<Transform>();

        

        public override void Init(Event_Data _data, EventCode eventCode)
        {
            base.Init(_data, eventCode);

            LoadEntity();

        }

        private void LoadEntity()
        {
            //RoleSoltPot.Add();

        }

        public override int GetRolePotToScendPot(Vector2 pot)
        {
            float dic = -1;

            int index = 0;
            for (int i = 0; i < RoleSoltPot.Count; i++)
            {
                Transform t = RoleSoltPot[i];
                Vector2 t_pot = CameraMgr.Instance.mainCamera.WorldToScreenPoint(t.position);
                float val = Vector2.Distance(t_pot, pot);
                if (dic < 0 || val<dic)
                {
                    dic = val;
                    index = i;
                }

            } 
            return index;
        }



    }
}
