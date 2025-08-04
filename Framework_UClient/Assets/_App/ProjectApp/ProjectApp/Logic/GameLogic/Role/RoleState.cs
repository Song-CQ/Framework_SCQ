using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;
using NUnit.Framework;
using UnityEditorInternal;
using UnityEngine;

namespace ProjectApp
{
    
    /// <summary>
    /// 角色在场景中的表现状态
    /// </summary>
    public enum RoleStateKey
    {
        待机 = 0,

        惊吓 = 10,
        幽灵,
        复活,

        相爱 = 20,
        心碎,
        拒绝,


    }

    /// <summary>
    /// 角色运行过的时间key
    /// </summary>
    public enum RunEventKey
    {
        /// <summary>
        /// 空
        /// </summary>
        Empty = 0,
        /// <summary>
        /// 相爱
        /// </summary>
        Love,
        /// <summary>
        /// 被拒绝相爱
        /// </summary>
        NoLove,
        /// <summary>
        /// 死亡
        /// </summary>
        Dead,
        /// <summary>
        /// 爱人死亡
        /// </summary>
        LoveDead

    }

    [Serializable]
    public class RoleState
    {

        // private Dictionary<StateKey, StateData> allState = new Dictionary<StateKey, StateData>();

        //修改成列表 按顺序来看状态
        private Dictionary<int,RunEventData> allRunEvents = new Dictionary<int,RunEventData>();
        
        private List<RoleStateKey> allState = new List<RoleStateKey>();
        private List<int> allStateData = new List<int>();

        public RoleStateKey GetCurrView()
        {
            RoleStateKey stateKey = RoleStateKey.待机;
            for (int i = 0; i < allState.Count; i++)
            {

                if (stateKey < allState[i])
                {
                    stateKey = allState[i];
                }
            }

            return stateKey;


        }

        


        public void AddRunEvent(int pictureIndex, RunEventKey key, int data = 0)
        {
            allRunEvents[pictureIndex] = new RunEventData(key, data);
        }

        public void AddState(RoleStateKey key, int data)
        {
            allState.Add(key);
            allStateData.Add(data);
        }
        public Dictionary<int,RunEventData> GetAllRunEvents()
        {
            return allRunEvents;
        }

        public bool GetState(RoleStateKey key, out List<int> datas)
        {
            datas = ListPool<int>.Get();
            for (int i = 0; i < allState.Count; i++)
            {
                if (allState[i] == key)
                {
                    datas.Add(allStateData[i]);
                }
            }

            if (datas.Count == 0)
            {
                ListPool<int>.Release(datas);
                datas = null;
                return false;
            }
            return true;

        }
        public List<RoleStateKey> GetAllState(out List<int> datas)
        {
            datas = allStateData;
            return allState;
        }


        

        public bool GetRunEvent(RunEventKey key, out RunEventData[] datas)
        {
            var dataList = ListPool<RunEventData>.Get();

            bool isHas = false;
            datas = null;

            foreach (var state in allRunEvents)
            {
                if (state.Value.stateKey == key)
                {
                    dataList.Add(state.Value);
                    isHas = true;

                }

            }
            if (isHas)
            {

                datas = dataList.ToArray();
                ListPool<RunEventData>.Release(dataList);
            }

            return isHas;
        }



        public void Rest()
        {

            allRunEvents.Clear();

            allState.Clear();
            allStateData.Clear();

        }

        public void CopyTo(RoleState state)
        {

            state.Rest();

            for (int i = 0; i < allState.Count; i++)
            {
                state.allState.Add(allState[i]);
                state.allStateData.Add(allStateData[i]);
            }

            foreach (var item in allRunEvents)
            {
                state.allRunEvents.Add(item.Key,item.Value);
            }

        }

        public string GetString()
        {
            string val = string.Empty;


            foreach (var state in allState)
            {
                // val = "StateKey: " + state.stateKey + "\n";
                // foreach (var item in state.Value.dataList_RoleKey)
                // {
                //     val += item.ToString() + "\n";
                // }

            }
            return val;

        }

        public bool GetDead()
        {
            bool isDead = false;
            foreach (var state in allState)
            {
                if (state == RoleStateKey.幽灵)
                {
                    isDead = true;
                }

            }
            return isDead;
        }

        
    }


    public struct RunEventData
    {

        public RunEventKey stateKey;
        public int data_int;

        public RunEventData(RunEventKey key, int data)
        {
            stateKey = key;
            data_int = data;
        }
    }
}
