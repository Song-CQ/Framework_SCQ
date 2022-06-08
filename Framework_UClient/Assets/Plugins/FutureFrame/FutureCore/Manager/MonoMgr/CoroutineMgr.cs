/****************************************************
    文件: NewClass.cs
    作者: Clear
    日期: 2022/6/7 11:40:37
    类型: 框架核心脚本(请勿修改)
    功能: 协程管理器
*****************************************************/
using System.Collections;
using System.Collections.Generic;

namespace FutureCore
{
    public class CoroutineMgr : BaseMonoMgr<CoroutineMgr>
    {
        private int id;
        private Dictionary<int, IEnumerator> allCoroutine;

        public override void Init()
        {
            base.Init();
            id = 0;
            allCoroutine = new Dictionary<int, IEnumerator>();
        }

        public int StartCoroutineToInt(IEnumerator coroutine)
        {
            id++;
            allCoroutine.Add(id,coroutine);
            StartCoroutine(DelayRomve(id,coroutine));
            return id;
        }

        public bool StopCoroutineToInt(int id)
        {
            if (allCoroutine.ContainsKey(id))
            {
                StopCoroutine(allCoroutine[id]);
                allCoroutine.Remove(id);
                return true;
            }
            return false;
        }

        private IEnumerator DelayRomve(int id, IEnumerator coroutine)
        {
            yield return StartCoroutine(coroutine);

            if (allCoroutine.ContainsKey(id))
            {
                allCoroutine.Remove(id);
            }
        }

    }
}