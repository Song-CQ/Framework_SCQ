using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionGameLibrary
{
    public class KeyState
    {
        public static string UP = "Up";
        public static string DOWN = "Down";
        public static string LEFT = "Left";
        public static string RIGHT = "Right";
        public static string JOY = "Joy";
        // 反转方向
        public static bool reverseDirection = false;

        private delegate bool DirAxisState(string key);

        private static Dictionary<string, DirAxisState> _dirKeyMap;
        private static Dictionary<string, DirAxisState> _dirKeyMap_Reverse;
        //手柄映射
        private static Dictionary<string, string> _joyKeyMap = new Dictionary<string, string>();
        //是否使用手柄
        // public static bool useJoyStick = false; 

        //键盘总开关
        public static bool inputEnable = true;
        //设置总体是否启用  by a5
        public static void setInputEnable(bool b)
        {
            inputEnable = b;
        }
        static KeyState()
        {
            _dirKeyMap = new Dictionary<string, DirAxisState>()
            {
                { UP, AxisPositive },
                { DOWN, AxisReversed },
                { LEFT, AxisReversed },
                { RIGHT, AxisPositive },
            };
            _dirKeyMap_Reverse = new Dictionary<string, DirAxisState>()
            {
                { UP, AxisReversed },
                { DOWN, AxisPositive },
                { LEFT, AxisPositive },
                { RIGHT, AxisReversed },
            };
            //
            for (int i = 0; i < KeyCode.codeP1.Count; i++)
            {
                _joyKeyMap.Add(KeyCode.codeP1[i], /*DeviceFactory.Instance.joyStr +*/ KeyCode.codeP1[i]);
            }

            for (int i = 0; i < KeyCode.codeP2.Count; i++)
            {
                _joyKeyMap.Add(KeyCode.codeP2[i], "none");
            }
        }

        public static bool IsDown(string key, bool reverseDirectionP = false)
        {
            if (!inputEnable) { return false; }
            //非编辑器不可使用键盘
#if !UNITY_EDITOR
            return false;
#endif

            bool isDown = false;
            DirAxisState func;

            if (_dirKeyMap.TryGetValue(key, out func))
            {

                isDown = reverseDirectionP ? _dirKeyMap_Reverse[key](key) : func(key);

            }
            else
            {
                try
                {
                    isDown = Input.GetButton(key) || Input.GetButton(_joyKeyMap[key]);
                }
                catch
                {
                    return false;
                }

            }
            return isDown;
        }


        private static bool AxisPositive(string key)
        {
            return Input.GetAxis(key) > 0.01f || Input.GetAxis(_joyKeyMap[key]) > 0.01;
        }

        private static bool AxisReversed(string key)
        {
            return Input.GetAxis(key) < -0.01f || Input.GetAxis(_joyKeyMap[key]) < -0.01f;
        }
    }
}