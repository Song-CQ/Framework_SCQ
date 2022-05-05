using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public static class MainShow 
    {

        public static void Show()
        {
            LogUtil.Log("怎在");
            FutureCore.CtrlDispatcher.Instance.AddListener(8000000, (o) =>
            {
                LogUtil.Log("成功了");

            });



        }

    }
}
