using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class SimpleTimer 
    {
        private Dictionary<Action, float> mIntervalDic = new Dictionary<Action, float>();
        private List<Action> triggers = new List<Action>();
        private float currTime;

        public SimpleTimer()
        {
            currTime = 0;
        }

        public void Update()
        {

            UpdateTime();

            if (mIntervalDic.Count > 0)
            {
                triggers.Clear();
                foreach (var task in mIntervalDic)
                {
                    if (GetTime() >= task.Value)
                    {
                        triggers.Add(task.Key);
                    }
                }

                if (triggers.Count > 0)
                {
                    foreach (var task in triggers)
                    {
                        mIntervalDic.Remove(task);
                        task?.Invoke();
                    }

                    triggers.Clear();
                }
            }
        }

        public void AddTimer(float interval, Action func)
        {
            if (func != null)
            {
                if (interval <= 0)
                {
                    func?.Invoke();
                    return;
                }

                if (!mIntervalDic.ContainsKey(func))
                {
                    mIntervalDic.Add(func, GetTime() + interval);
                }
            }

        }

        public void ClearAll()
        {
            mIntervalDic.Clear();
            triggers.Clear();
        }

        public void Dispose()
        {
            ClearAll();
            mIntervalDic = null;
            triggers = null;
        }

        public void RemoveTimer(Action func)
        {
            if (func != null)
            {
                if (mIntervalDic.ContainsKey(func))
                {
                    mIntervalDic.Remove(func);
                }
            }

        }

        private float GetTime()
        {
            return currTime;
        }
        private void UpdateTime()
        {
            currTime += Time.deltaTime;
        }

    }
}