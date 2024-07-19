/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

/*
 Author:du
 Time:2017.10.12
*/

using UnityEngine;

namespace FutureCore
{
    public static class YieldConst
    {
        public const float Time10ms = 0.01f;
        public const float Time100ms = 0.1f;
        public const float Time300ms = 0.3f;
        public const float Time500ms = 0.5f;
        public const float Time1s = 1f;
        public const float Time3s = 3f;
        public const float Time5s = 5f;

        public const object WaitForNextFrame = null;
        public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        public static WaitForSeconds WaitFor10ms = new WaitForSeconds(Time10ms);
        public static WaitForSeconds WaitFor100ms = new WaitForSeconds(Time100ms);
        public static WaitForSeconds WaitFor300ms = new WaitForSeconds(Time300ms);
        public static WaitForSeconds WaitFor500ms = new WaitForSeconds(Time500ms);
        public static WaitForSeconds WaitFor1s = new WaitForSeconds(Time1s);
        public static WaitForSeconds WaitFor3s = new WaitForSeconds(Time3s);
        public static WaitForSeconds WaitFor5s = new WaitForSeconds(Time5s);
        public static WaitForSecondsRealtime WaitForRealtime1s = new WaitForSecondsRealtime(1f);
    }
}