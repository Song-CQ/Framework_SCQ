using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class RoleState
    {
       
        private Dictionary<StateKey, StateData> allState = new Dictionary<StateKey, StateData>();


        public void SetState(StateKey key, StateData data)
        {
            allState[key] = data;

        }

        public void Rest()
        {
            allState.Clear();

        }

        public void CopyTo(RoleState state)
        {
            



        }
    }


    public class StateData
    {
        public int key;
    }
}
