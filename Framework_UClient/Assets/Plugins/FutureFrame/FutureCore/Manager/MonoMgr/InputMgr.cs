using System;
using UnityEngine;
namespace FutureCore
{
    public sealed class InputMgr : BaseMonoMgr<InputMgr>
    {
        public static event Action<Vector2> ClickScreen;

        public static event Action UpData;
        public static event Action UpData_Second;

        private float timeTemp_Second = 0;

        public override void Init()
        {
            base.Init();
            UpData+=MainThreadLog.LoopLog;
        }

        private void Update()
        {
            if (!IsStartUp||IsDispose)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                ClickScreen?.Invoke(Input.mousePosition);
            }
            UpData?.Invoke();
            timeTemp_Second += Time.deltaTime;
            if (timeTemp_Second >= 1)
            {
                timeTemp_Second = 0;
                UpData_Second?.Invoke();
            }
        }
        
    }

}