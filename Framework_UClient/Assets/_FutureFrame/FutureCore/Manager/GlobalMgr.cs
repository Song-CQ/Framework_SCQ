/****************************************************************************
* ScriptType: �����
* ����������
****************************************************************************/
using System.Collections.Generic;

namespace FutureCore
{
    public class GlobalMgr : Singleton<GlobalMgr>
    {
        private bool isLog = true;

        private List<IMgr> allMgr = new List<IMgr>();

        public void StartUp()
        {
            LogUtil.Log("[GlobalMgr]StartUp Start");
            foreach (var mgr in allMgr)
            {
                mgr.Init();
            }
            foreach (var mgr in allMgr)
            {
                mgr.StartUp();
            }
   
            AppDispatcher.Instance.Dispatch(AppMsg.System_ManagerStartUpComplete);
            LogUtil.Log("[GlobalMgr]StartUp End");
        }

        /// <summary>
        ///  检测是否有未StartUp的Imgr
        /// </summary>
        /// <param name="isOpen"> 如果未StartUp 是否将它启动</param>
        /// <returns></returns>
        public bool CheckStartUp(bool isOpen = true)
        {
            LogUtil.Log("[GlobalMgr]Check StartUp Start");
            bool isface = false;
            foreach (var item in allMgr)
            {
                if (!item.IsInit&&!isface)
                {
                    isface = true;
                }
                if (!item.IsStartUp && isOpen)
                {
                    item.Init();
                    if (isLog)
                    {
                        LogUtil.Log("[GlobalMgr]" + item.GetType().Name + "---- Init".AddColor(UnityEngine.Color.yellow));
                    }
                    
                }
            }
            foreach (var item in allMgr)
            {
                if (!item.IsStartUp && !isface)
                {
                    isface = true;
                }
                if (!item.IsStartUp && isOpen)
                {
                    item.StartUp();
                    if (isLog)
                    {
                        LogUtil.Log("[GlobalMgr]" + item.GetType().Name + "---- StartUp".AddColor(UnityEngine.Color.yellow));
                    }

                }
            }
            LogUtil.Log("[GlobalMgr]Check StartUp End");
            return isface;
        }

        public void AddMgr(IMgr mgr)
        {
            if (!allMgr.Contains(mgr))
            {
                allMgr.Add(mgr);
            }
        }

        public void DisposeAllMgr()
        {
            foreach (IMgr mgr in allMgr)
            {
                mgr.DisposeBefore();
            }
            foreach (IMgr mgr in allMgr)
            {
                mgr.Dispose();
            }
            EngineUtil.Destroy(AppObjConst.MonoManagerGo);
            allMgr.Clear();
        }

        public override void Dispose()
        {
            base.Dispose();
            allMgr.Clear();
            allMgr = null;
        }

    }


}
