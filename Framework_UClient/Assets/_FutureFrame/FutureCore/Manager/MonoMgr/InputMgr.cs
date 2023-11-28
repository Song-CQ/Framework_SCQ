using System;
using UnityEngine;
namespace FutureCore
{
    public sealed class InputMgr : BaseMonoMgr<InputMgr>
    {
        public static event Action<Vector2> ClickScreen;      

        public override void Init()
        {
            base.Init();
           
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
          
        }
        
    }

}