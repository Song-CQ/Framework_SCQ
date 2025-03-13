using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class RoleState
    {

        private Dictionary<StateKey, StateData> allState = new Dictionary<StateKey, StateData>();


        public void SetState(StateKey key, StateData data = default)
        {
            allState[key] = data;

        }

        public bool GetState(StateKey key, out StateData data)
        {
            data = default;
            bool isHas = false;
            if (allState.ContainsKey(key))
            {
                data = allState[key];
                isHas = true;
            }

            return isHas;
        }

        public bool GetState(StateKey key)
        {
           return allState.ContainsKey(key);
        }

        public void Rest()
        {
            foreach (var item in allState)
            {
                item.Value.Rest();
            }
            allState.Clear();

        }

        public void CopyTo(RoleState state)
        {

            state.Rest();

            foreach (var item in allState)
            {
                StateData stateData = default;

                stateData.data_int = item.Value.data_int;

                if (item.Value.dataList_RoleKey!=null)
                {
                    stateData.dataList_RoleKey = new List<RoleKey>(item.Value.dataList_RoleKey.Capacity);

                    foreach (var data in item.Value.dataList_RoleKey)
                    {
                        stateData.dataList_RoleKey.Add(data);
                    } 
                }

                state.allState.Add(item.Key, stateData);


            }

        }

        public string GetString()
        {
            string val = string.Empty;


            foreach (var state in allState)
            {
                val = "StateKey: " + state.Key+ "\n";
                foreach (var item in state.Value.dataList_RoleKey)
                {
                    val += item.ToString()+"\n";
                }
                            
            }
            return val;

        }
    }


    public struct StateData
    {
        public int data_int;
        public List<RoleKey> dataList_RoleKey;


        public void Rest()
        {
            data_int = 0;

            dataList_RoleKey = null;
        }
    }
}
