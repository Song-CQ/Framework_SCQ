using UnityEngine;
using System.Collections.Generic;

namespace ActionGameLibrary
{
    public class VirtualKeyState
    {
        public const string UP = "Up";
        public const string DOWN = "Down";
        public const string LEFT = "Left";
        public const string RIGHT = "Right";
        public const string A = "Fire1";
        public const string B = "Jump";
        public const string SLOT_1 = "Slot1";
        public const string SLOT_2 = "Slot2";
        public const string SLOT_3 = "Slot3";
        public const string SLOT_4 = "Slot4";
        public const string SLOT_5 = "Slot5";
        public const string SLOT_6 = "Slot6";
        public const string SLOT_7 = "Slot7";
        public const string SLOT_8 = "Slot8";
        public const string SLOT_9 = "Slot9";
        public const string SLOT_10 = "Slot10";
        public const string SLOT_11 = "Slot11";
        public const string SLOT_12 = "Slot12";
        public const string SLOT_13 = "Slot13";
        public const string SLOT_14 = "Slot14";
        public const string SLOT_15 = "Slot15";

        public const string FUN_1 = "Function1";
        public const string FUN_2 = "Function2";
        public const string FUN_3 = "Function3";
        public const string FUN_4 = "Function4";
        public const string FUN_5 = "Function5";
        public const string FUN_6 = "Function6";
        public const string FUN_7 = "Function7";
        
        public const string Roulette_0 = "Roulette0";
        public const string Roulette_1 = "Roulette1";
        public const string Roulette_2 = "Roulette2";
        public const string Roulette_3 = "Roulette3";
        public const string Roulette_4 = "Roulette4";

        public const string LEFT_UP = "Left_Up";
        public const string LEFT_DOWN = "Left_Down";
        public const string RIGHT_UP = "Right_Up";
        public const string RIGHT_DOWN = "Right_Down";

        public const string UP_2P = "Up_2p";
        public const string DOWN_2P = "Down_2p";
        public const string LEFT_2P = "Left_2p";
        public const string RIGHT_2P = "Right_2p";
        public const string A_2P = "Fire1_2p";
        public const string B_2P = "Jump_2p";
        public const string SLOT_1_2P = "Slot1_2p";
        public const string SLOT_2_2P = "Slot2_2p";
        public const string SLOT_3_2P = "Slot3_2p";
        public const string SLOT_4_2P = "Slot4_2p";
        public const string SLOT_5_2P = "Slot5_2p";
        public const string SLOT_6_2P = "Slot6_2p";
        public const string SLOT_7_2P = "Slot7_2p";
        public const string SLOT_8_2P = "Slot8_2p";
        public const string SLOT_9_2P = "Slot9_2p";
        public const string SLOT_10_2P = "Slot10_2p";
        public const string SLOT_11_2P = "Slot11_2p";
        public const string SLOT_12_2P = "Slot12_2p";
        public const string SLOT_13_2P = "Slot13_2p";
        public const string SLOT_14_2P = "Slot14_2p";
        public const string SLOT_15_2P = "Slot15_2p";
        public const string FUN_1_2P = "Function1_2P";
        public const string FUN_2_2P = "Function2_2P";
        public const string FUN_3_2P = "Function3_2P";
        public const string FUN_4_2P = "Function4_2P";
        public const string FUN_5_2P = "Function5_2P";
        public const string FUN_6_2P = "Function6_2P";
        public const string FUN_7_2P = "Function7_2P";

        public const string Roulette_0_2p = "Roulette0_2p";
        public const string Roulette_1_2p = "Roulette1_2p";
        public const string Roulette_2_2p = "Roulette2_2p";
        public const string Roulette_3_2p = "Roulette3_2p";
        public const string Roulette_4_2p = "Roulette4_2p";


        public const string LEFT_UP_2P = "Left_Up_2p";
        public const string LEFT_DOWN_2P = "Left_Down_2p";
        public const string RIGHT_UP_2P = "Right_Up_2p";
        public const string RIGHT_DOWN_2P = "Right_Down_2p";

        public const int UP_ANGLE_MIN = 70;
        public const int UP_ANGLE_MAX = 110;
        public const int DOWN_ANGLE_MIN = 250;
        public const int DOWN_ANGLE_MAX = 290;
        public const int LEFT_ANGLE_MIN = 150;
        public const int LEFT_ANGLE_MAX = 210;
        public const int RIGHT_ANGLE_MIN = 30;
        public const int RIGHT_ANGLE_MAX = 330;



        public static int lockDirection = 0;
        public static bool keyEnable = true;
        public static bool joystickEnable = true;

        private static float _joyAngle = 0;
        private static float _joyPower = 0;


        private static Dictionary<string, bool> _virtualKeyState = new Dictionary<string, bool>()
        {
            {VirtualKeyState.UP,false},
            {VirtualKeyState.DOWN,false},
            {VirtualKeyState.LEFT,false},
            {VirtualKeyState.RIGHT,false},
            {VirtualKeyState.A,false},
            {VirtualKeyState.B,false},
            {VirtualKeyState.SLOT_1,false},
            {VirtualKeyState.SLOT_2,false},
            {VirtualKeyState.SLOT_3,false},
            {VirtualKeyState.SLOT_4,false},
            {VirtualKeyState.SLOT_5,false},
            {VirtualKeyState.SLOT_6,false},
            {VirtualKeyState.SLOT_7,false},
            {VirtualKeyState.SLOT_8,false},
            {VirtualKeyState.SLOT_9,false},
            {VirtualKeyState.SLOT_10,false},
            {VirtualKeyState.SLOT_11,false},
            {VirtualKeyState.SLOT_12,false},
            {VirtualKeyState.SLOT_13,false},
            {VirtualKeyState.SLOT_14,false},
            {VirtualKeyState.SLOT_15,false},
            {VirtualKeyState.FUN_1,false},
            {VirtualKeyState.FUN_2,false},
            {VirtualKeyState.FUN_3,false},
            {VirtualKeyState.FUN_4,false},
            {VirtualKeyState.FUN_5,false},
            {VirtualKeyState.FUN_6,false},
            {VirtualKeyState.FUN_7,false},

            {VirtualKeyState.Roulette_0,false},
            {VirtualKeyState.Roulette_1,false},
            {VirtualKeyState.Roulette_2,false},
            {VirtualKeyState.Roulette_3,false},
            {VirtualKeyState.Roulette_4,false},

            {VirtualKeyState.LEFT_UP,false},
            {VirtualKeyState.LEFT_DOWN,false},
            {VirtualKeyState.RIGHT_UP,false},
            {VirtualKeyState.RIGHT_DOWN,false},

            {VirtualKeyState.UP_2P,false},
            {VirtualKeyState.DOWN_2P,false},
            {VirtualKeyState.LEFT_2P,false},
            {VirtualKeyState.RIGHT_2P,false},
            {VirtualKeyState.A_2P,false},
            {VirtualKeyState.B_2P,false},
            {VirtualKeyState.SLOT_1_2P,false},
            {VirtualKeyState.SLOT_2_2P,false},
            {VirtualKeyState.SLOT_3_2P,false},
            {VirtualKeyState.SLOT_4_2P,false},
            {VirtualKeyState.SLOT_5_2P,false},
            {VirtualKeyState.SLOT_6_2P,false},
            {VirtualKeyState.SLOT_7_2P,false},
            {VirtualKeyState.SLOT_8_2P,false},
            {VirtualKeyState.SLOT_9_2P,false},
            {VirtualKeyState.SLOT_10_2P,false},
            {VirtualKeyState.SLOT_11_2P,false},
            {VirtualKeyState.SLOT_12_2P,false},
            {VirtualKeyState.SLOT_13_2P,false},
            {VirtualKeyState.SLOT_14_2P,false},
            {VirtualKeyState.SLOT_15_2P,false},
            {VirtualKeyState.FUN_1_2P,false},
            {VirtualKeyState.FUN_2_2P,false},
            {VirtualKeyState.FUN_3_2P,false},
            {VirtualKeyState.FUN_4_2P,false},
            {VirtualKeyState.FUN_5_2P,false},
            {VirtualKeyState.FUN_6_2P,false},
            {VirtualKeyState.FUN_7_2P,false},

            {VirtualKeyState.Roulette_0_2p,false},
            {VirtualKeyState.Roulette_1_2p,false},
            {VirtualKeyState.Roulette_2_2p,false},
            {VirtualKeyState.Roulette_3_2p,false},
            {VirtualKeyState.Roulette_4_2p,false},


            {VirtualKeyState.LEFT_UP_2P,false},
            {VirtualKeyState.LEFT_DOWN_2P,false},
            {VirtualKeyState.RIGHT_UP_2P,false},
            {VirtualKeyState.RIGHT_DOWN_2P,false}
        };

        private static Dictionary<string, string> _keyReverseMap = new Dictionary<string, string>()
        {
            {VirtualKeyState.UP,VirtualKeyState.DOWN},
            {VirtualKeyState.DOWN,VirtualKeyState.UP},
            {VirtualKeyState.LEFT,VirtualKeyState.RIGHT},
            {VirtualKeyState.RIGHT,VirtualKeyState.LEFT},

            {VirtualKeyState.LEFT_UP,VirtualKeyState.RIGHT_DOWN},
            {VirtualKeyState.LEFT_DOWN,VirtualKeyState.RIGHT_UP},
            {VirtualKeyState.RIGHT_UP,VirtualKeyState.LEFT_DOWN},
            {VirtualKeyState.RIGHT_DOWN,VirtualKeyState.LEFT_UP}
        };

        public static float joyAngle
        {
            get { return _joyAngle; }
        }

        public static float joyPower
        {
            get { return _joyPower; }
        }

        //private StarsEngine _engine;
        ////摇杆
        //public delegate void DG_Joystick(int angle, float power);
        ////按键

        public static bool IsDown(string key, bool reverseDirection)
        {
            if (!_virtualKeyState.ContainsKey(key)) LogUtil.LogError(key);

            if (reverseDirection)
            {
                string newKey;
                bool has = _keyReverseMap.TryGetValue(key, out newKey);
                if (has) key = newKey;
            }
            return _virtualKeyState[key];
        }

        public static void Reset()
        {
            _virtualKeyState.Clear();
            _virtualKeyState[VirtualKeyState.UP] = false;
            _virtualKeyState[VirtualKeyState.DOWN] = false;
            _virtualKeyState[VirtualKeyState.LEFT] = false;
            _virtualKeyState[VirtualKeyState.RIGHT] = false;
            _virtualKeyState[VirtualKeyState.A] = false;
            _virtualKeyState[VirtualKeyState.B] = false;
            _virtualKeyState[VirtualKeyState.SLOT_1] = false;
            _virtualKeyState[VirtualKeyState.SLOT_2] = false;
            _virtualKeyState[VirtualKeyState.SLOT_3] = false;
            _virtualKeyState[VirtualKeyState.SLOT_4] = false;
            _virtualKeyState[VirtualKeyState.SLOT_5] = false;
            _virtualKeyState[VirtualKeyState.SLOT_6] = false;
            _virtualKeyState[VirtualKeyState.SLOT_7] = false;
            _virtualKeyState[VirtualKeyState.SLOT_8] = false;
            _virtualKeyState[VirtualKeyState.SLOT_9] = false;
            _virtualKeyState[VirtualKeyState.SLOT_10] = false;
            _virtualKeyState[VirtualKeyState.SLOT_11] = false;
            _virtualKeyState[VirtualKeyState.SLOT_12] = false;
            _virtualKeyState[VirtualKeyState.SLOT_13] = false;
            _virtualKeyState[VirtualKeyState.SLOT_14] = false;
            _virtualKeyState[VirtualKeyState.SLOT_15] = false;
            _virtualKeyState[VirtualKeyState.FUN_1] = false;
            _virtualKeyState[VirtualKeyState.FUN_2] = false;
            _virtualKeyState[VirtualKeyState.FUN_3] = false;
            _virtualKeyState[VirtualKeyState.FUN_4] = false;
            _virtualKeyState[VirtualKeyState.FUN_5] = false;
            _virtualKeyState[VirtualKeyState.FUN_6] = false;
            _virtualKeyState[VirtualKeyState.FUN_7] = false;

            _virtualKeyState[VirtualKeyState.Roulette_0] = false;
            _virtualKeyState[VirtualKeyState.Roulette_1] = false;
            _virtualKeyState[VirtualKeyState.Roulette_2] = false;
            _virtualKeyState[VirtualKeyState.Roulette_3] = false;
            _virtualKeyState[VirtualKeyState.Roulette_4] = false;

            _virtualKeyState[VirtualKeyState.LEFT_UP] = false;
            _virtualKeyState[VirtualKeyState.LEFT_DOWN] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_UP] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_DOWN] = false;

            _virtualKeyState[VirtualKeyState.UP_2P] = false;
            _virtualKeyState[VirtualKeyState.DOWN_2P] = false;
            _virtualKeyState[VirtualKeyState.LEFT_2P] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_2P] = false;
            _virtualKeyState[VirtualKeyState.A_2P] = false;
            _virtualKeyState[VirtualKeyState.B_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_1_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_2_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_3_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_4_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_5_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_6_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_7_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_8_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_9_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_10_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_11_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_12_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_13_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_14_2P] = false;
            _virtualKeyState[VirtualKeyState.SLOT_15_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_1_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_2_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_3_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_4_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_5_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_6_2P] = false;
            _virtualKeyState[VirtualKeyState.FUN_7_2P] = false;


            _virtualKeyState[VirtualKeyState.LEFT_UP_2P] = false;
            _virtualKeyState[VirtualKeyState.LEFT_DOWN_2P] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_UP_2P] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_DOWN_2P] = false;

            _virtualKeyState[VirtualKeyState.Roulette_0_2p] = false;
            _virtualKeyState[VirtualKeyState.Roulette_1_2p] = false;
            _virtualKeyState[VirtualKeyState.Roulette_2_2p] = false;
            _virtualKeyState[VirtualKeyState.Roulette_3_2p] = false;
            _virtualKeyState[VirtualKeyState.Roulette_4_2p] = false;

        }
        private static void ReDirection()
        {
            _virtualKeyState[VirtualKeyState.UP] = false;
            _virtualKeyState[VirtualKeyState.DOWN] = false;
            _virtualKeyState[VirtualKeyState.LEFT] = false;
            _virtualKeyState[VirtualKeyState.RIGHT] = false;

            _virtualKeyState[VirtualKeyState.LEFT_UP] = false;
            _virtualKeyState[VirtualKeyState.LEFT_DOWN] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_UP] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_DOWN] = false;

            _virtualKeyState[VirtualKeyState.UP_2P] = false;
            _virtualKeyState[VirtualKeyState.DOWN_2P] = false;
            _virtualKeyState[VirtualKeyState.LEFT_2P] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_2P] = false;

            _virtualKeyState[VirtualKeyState.LEFT_UP_2P] = false;
            _virtualKeyState[VirtualKeyState.LEFT_DOWN_2P] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_UP_2P] = false;
            _virtualKeyState[VirtualKeyState.RIGHT_DOWN_2P] = false;

            _joyAngle = 0;
            _joyPower = 0;
        }

        //设置按键状态
        public static void SetKeyDown(string key, bool isDown)
        {
            if (!keyEnable)
                return;
            _virtualKeyState[key] = isDown;
        }
        //设置摇杆角度
        public static void setJoystickState(int angle, float power)
        {
            ReDirection();
            //Log.info(angle + "," + power);
            if (power > 0f)
            {
                SwitchAngle(angle);
            }
            _joyAngle = angle - 90;
            _joyPower = power;
        }

        private const float step = 360f / 16f;
        //对应16个区域，逆时针开始所对应的key开关
        private static List<string[]> rangeDict = new List<string[]>{
            new string[]{  VirtualKeyState.RIGHT } ,
            new string[]{  VirtualKeyState.RIGHT, VirtualKeyState.RIGHT_UP } ,
            new string[]{  VirtualKeyState.RIGHT_UP} ,
            new string[]{  VirtualKeyState.RIGHT_UP,VirtualKeyState.UP } ,
            new string[]{  VirtualKeyState.UP } ,
            new string[]{  VirtualKeyState.UP,VirtualKeyState.LEFT_UP } ,
            new string[]{  VirtualKeyState.LEFT_UP } ,
            new string[]{  VirtualKeyState.LEFT_UP,  VirtualKeyState.LEFT} ,
            new string[]{  VirtualKeyState.LEFT } ,
            new string[]{  VirtualKeyState.LEFT,  VirtualKeyState.LEFT_DOWN} ,
            new string[]{  VirtualKeyState.LEFT_DOWN } ,
            new string[]{  VirtualKeyState.LEFT_DOWN,  VirtualKeyState.DOWN} ,
            new string[]{  VirtualKeyState.DOWN } ,
            new string[]{  VirtualKeyState.DOWN,  VirtualKeyState.RIGHT_DOWN} ,
            new string[]{  VirtualKeyState.RIGHT_DOWN } ,
            new string[]{  VirtualKeyState.RIGHT_DOWN,  VirtualKeyState.RIGHT}
        };

        private static void SwitchAngle(float angle)
        {
            angle = -angle + 90;
            if (angle < 0f)
            {
                angle += 360f;
            }

            //把最终运算的区域偏移0.5个step，这是因为 右 的有一半区域在四象限
            int index = Mathf.FloorToInt(angle / step + 0.5f);
            if (index == 16) { index = 0; }

            string[] key = rangeDict[index];
            for (int i = 0; i < key.Length; i++)
            {
                _virtualKeyState[key[i]] = true;
                //Log.info(key[i]);
            }


            // bool isDown = angle != 90;

            //if (angle == 90)
            //Log.infoError(angle==90);
            // 右
            //if (angle > RIGHT_ANGLE_MAX || angle < RIGHT_ANGLE_MIN)
            //{
            //    //if (isDown && isUp)
            //    _virtualKeyState[VirtualKeyState.RIGHT] = true;
            //}
            //else if (angle >= RIGHT_ANGLE_MIN && angle <= UP_ANGLE_MIN)
            //{
            //    // 右上
            //    // if (isDown && isUp)
            //    _virtualKeyState[VirtualKeyState.UP] = true;
            //    _virtualKeyState[VirtualKeyState.RIGHT] = true;
            //}
            //else if (angle > UP_ANGLE_MIN && angle < UP_ANGLE_MAX)
            //{
            //    // 上
            //    _virtualKeyState[VirtualKeyState.UP] = true;
            //}
            //else if (angle >= UP_ANGLE_MAX && angle <= LEFT_ANGLE_MIN)
            //{
            //    // 左上
            //    //  if (isDown && isUp)
            //    _virtualKeyState[VirtualKeyState.UP] = true;
            //    _virtualKeyState[VirtualKeyState.LEFT] = true;
            //}
            //else if (angle > LEFT_ANGLE_MIN && angle < LEFT_ANGLE_MAX)
            //{
            //    // 左
            //    // if (isDown && isUp)
            //    _virtualKeyState[VirtualKeyState.LEFT] = true;
            //}
            //else if (angle >= LEFT_ANGLE_MAX && angle <= DOWN_ANGLE_MIN)
            //{
            //    // 左下
            //    //if (isDown && isUp)
            //    _virtualKeyState[VirtualKeyState.LEFT] = true;
            //    _virtualKeyState[VirtualKeyState.DOWN] = true;
            //}
            //else if (angle > DOWN_ANGLE_MIN && angle < DOWN_ANGLE_MAX)
            //{
            //    // 下
            //    _virtualKeyState[VirtualKeyState.DOWN] = true;
            //}
            //else if (angle >= DOWN_ANGLE_MAX && angle <= RIGHT_ANGLE_MAX)
            //{
            //    // 右下
            //    //    if (isDown && isUp)
            //    _virtualKeyState[VirtualKeyState.RIGHT] = true;
            //    _virtualKeyState[VirtualKeyState.DOWN] = true;
            //}
        }
    }
}
