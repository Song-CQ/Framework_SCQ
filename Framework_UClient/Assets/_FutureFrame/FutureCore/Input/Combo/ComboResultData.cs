
using System;
using UnityEngine;

namespace FutureCore
{
    public class ComboResultData : IObjectPoolItem
    {
        public string combo;
        public int priority;
        public int extendFrame;
        public string lastKeyCode;

        public ComboResultData()
        {
        }

        public ComboResultData(ComboData cd)
        {
            Fill(cd);
        }

        public void Dispose()
        {
            Reset();
        }

        public void Fill(ComboData cd)
        {
            combo = cd.combo;
            priority = cd.priority;
            extendFrame = cd.extendFrame;
            InputOrderData order = cd.ordweList[cd.ordweList.Count - 1] as InputOrderData;
            lastKeyCode = order.code;
        }

        public void Fill(ComboResultData crd)
        {
            combo = crd.combo;
            priority = crd.priority;
            extendFrame = crd.extendFrame;
            lastKeyCode = crd.lastKeyCode;
        }

        public void Reset()
        {
            combo = null;
            priority = 0;
            extendFrame = 0;
            lastKeyCode = null;
        }
    }
}

