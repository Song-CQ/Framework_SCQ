/****************************************************************************
* ScriptType: Ö÷¿ò¼Ü
* ¹ÜÀíÆ÷ÈÝÆ÷
****************************************************************************/
using System.Collections.Generic;

namespace FutureCore
{
    public class GlobalMgr : Singleton<GlobalMgr>
    {
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
           //ModuleMgr.Instance.StartUpAllModule();
            AppDispatcher.Instance.Dispatch(AppMsg.System_ManagerStartUpComplete);
            LogUtil.Log("[GlobalMgr]StartUp End");
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
