using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ActionGameLibrary
{
    public class KeyCode
    {
        public const string KEY_LABEL_UP = "up";
        public const string KEY_LABEL_DOWN = "down";
        public const string KEY_LABEL_LEFT = "left";
        public const string KEY_LABEL_RIGHT = "right";
        public const string KEY_LABEL_A = "a";
        public const string KEY_LABEL_B = "b";
        public const string KEY_LABEL_S1 = "s1";
        public const string KEY_LABEL_S2 = "s2";
        public const string KEY_LABEL_S3 = "s3";
        public const string KEY_LABEL_S4 = "s4";
        public const string KEY_LABEL_S5 = "s5";
        public const string KEY_LABEL_S6 = "s6";
        public const string KEY_LABEL_S7 = "s7";
        public const string KEY_LABEL_S8 = "s8";
        public const string KEY_LABEL_S9 = "s9";
        public const string KEY_LABEL_S10 = "s10";
        public const string KEY_LABEL_S11 = "s11";
        public const string KEY_LABEL_S12 = "s12";
        public const string KEY_LABEL_S13 = "s13";
        public const string KEY_LABEL_S14 = "s14";
        public const string KEY_LABEL_S15 = "s15";
        public const string KEY_LABEL_FUN1 = "fun1";
        public const string KEY_LABEL_FUN2 = "fun2";
        public const string KEY_LABEL_FUN3 = "fun3";
        public const string KEY_LABEL_FUN4 = "fun4";
        public const string KEY_LABEL_FUN5 = "fun5";
        public const string KEY_LABEL_FUN6 = "fun6";
        public const string KEY_LABEL_FUN7 = "fun7";
        
        public const string KEY_LABEL_ROULETTE0 = "roulette0";
        public const string KEY_LABEL_ROULETTE1 = "roulette1";
        public const string KEY_LABEL_ROULETTE2 = "roulette2";
        public const string KEY_LABEL_ROULETTE3 = "roulette3";
        public const string KEY_LABEL_ROULETTE4 = "roulette4";

        //
        public const string KEY_LABEL_LEFT_UP = "left_up";
        public const string KEY_LABEL_LEFT_DOWN = "left_down";
        public const string KEY_LABEL_RIGHT_UP = "right_up";
        public const string KEY_LABEL_RIGHT_DOWN = "right_down";

        public static Dictionary<string, int> reverseIndexMap = new Dictionary<string, int>
        {
            {KEY_LABEL_UP,0},
            {KEY_LABEL_DOWN,1},
            {KEY_LABEL_LEFT,2},
            {KEY_LABEL_RIGHT,3},
            {KEY_LABEL_A,4},
            {KEY_LABEL_B,5},
            {KEY_LABEL_S1,6},
            {KEY_LABEL_S2,7},
            {KEY_LABEL_S3,8},
            {KEY_LABEL_S4,9},
            {KEY_LABEL_S5,10},
            {KEY_LABEL_S6,11},
            {KEY_LABEL_S7,12},
            {KEY_LABEL_S8,13},
            {KEY_LABEL_S9,14},
            {KEY_LABEL_S10,15},
            {KEY_LABEL_S11,16},
            {KEY_LABEL_S12,17},
            {KEY_LABEL_S13,18},
            {KEY_LABEL_S14,19},
            {KEY_LABEL_S15,20},
            {KEY_LABEL_FUN1,21},
            {KEY_LABEL_FUN2,22},
            {KEY_LABEL_FUN3,23},
            {KEY_LABEL_FUN4,24},
            {KEY_LABEL_FUN5,25},
            {KEY_LABEL_FUN6,26},
            {KEY_LABEL_LEFT_UP,27},
            {KEY_LABEL_LEFT_DOWN,28},
            {KEY_LABEL_RIGHT_UP,29},
            {KEY_LABEL_RIGHT_DOWN,30},
            {KEY_LABEL_FUN7,31},
            {KEY_LABEL_ROULETTE0,32},
            {KEY_LABEL_ROULETTE1,33},
            {KEY_LABEL_ROULETTE2,34},
            {KEY_LABEL_ROULETTE3,35},
            {KEY_LABEL_ROULETTE4,36},

        };

        public const int KEY_STATE_UP = 1 << 0;
        public const int KEY_STATE_DOWN = 1 << 1;
        public const int KEY_STATE_LEFT = 1 << 2;
        public const int KEY_STATE_RIGHT = 1 << 3;
        public const int KEY_STATE_A = 1 << 4;
        public const int KEY_STATE_B = 1 << 5;
        public const int KEY_SLOT_1 = 1 << 6;
        public const int KEY_SLOT_2 = 1 << 7;
        public const int KEY_SLOT_3 = 1 << 8;
        public const int KEY_SLOT_4 = 1 << 9;
        public const int KEY_SLOT_5 = 1 << 10;
        public const int KEY_SLOT_6 = 1 << 11;
        public const int KEY_SLOT_7 = 1 << 12;
        public const int KEY_SLOT_8 = 1 << 13;
        public const int KEY_SLOT_9 = 1 << 14;
        public const int KEY_SLOT_10 = 1 << 15;
        public const int KEY_SLOT_11 = 1 << 16;
        public const int KEY_SLOT_12 = 1 << 17;
        public const int KEY_SLOT_13 = 1 << 18;
        public const int KEY_SLOT_14 = 1 << 19;
        public const int KEY_SLOT_15 = 1 << 20;
        public const int FUNCTION_1 = 1 << 21;
        public const int FUNCTION_2 = 1 << 22;
        public const int FUNCTION_3 = 1 << 23;
        public const int FUNCTION_4 = 1 << 24;
        public const int FUNCTION_5 = 1 << 25;
        public const int FUNCTION_6 = 1 << 26;
        public const long FUNCTION_7 = 1L << 31;

        public const long ROULETTE0 = 1L << 32;
        public const long ROULETTE1 = 1L << 33;
        public const long ROULETTE2 = 1L << 34;
        public const long ROULETTE3 = 1L << 35;
        public const long ROULETTE4 = 1L << 36;


        //增加4个方向，让组合变为 16个方向
        public const int KEY_EXDIR_LEFT_UP = 1 << 27;
        public const int KEY_EXDIR_LEFT_DOWN = 1 << 28;
        public const int KEY_EXDIR_RIGHT_UP = 1 << 29;
        public const int KEY_EXDIR_RIGHT_DOWN = 1 << 30;

        public enum MoveDirection
        {
            D1 = 2 | 4,
            D2 = 2,
            D3 = 2 | 8,
            D4 = 4,
            D5 = 0,
            D6 = 8,
            D7 = 1 | 4,
            D8 = 1,
            D9 = 1 | 8
            //
        }


        public static long[] KeyKtates = new long[] { KEY_STATE_UP, KEY_STATE_DOWN, KEY_STATE_LEFT, KEY_STATE_RIGHT, KEY_STATE_A, KEY_STATE_B,
            KEY_SLOT_1, KEY_SLOT_2, KEY_SLOT_3, KEY_SLOT_4, KEY_SLOT_5, KEY_SLOT_6, KEY_SLOT_7, KEY_SLOT_8, KEY_SLOT_9,KEY_SLOT_10,KEY_SLOT_11,KEY_SLOT_12,KEY_SLOT_13,KEY_SLOT_14,KEY_SLOT_15,
            FUNCTION_1, FUNCTION_2, FUNCTION_3, FUNCTION_4,FUNCTION_5,FUNCTION_6,FUNCTION_7,
            ROULETTE0,ROULETTE1,ROULETTE2,ROULETTE3,ROULETTE4,
            KEY_EXDIR_LEFT_UP, KEY_EXDIR_LEFT_DOWN, KEY_EXDIR_RIGHT_UP, KEY_EXDIR_RIGHT_DOWN };

        public static int codeLength = 11;
        public static List<string> label = new List<string>() { KEY_LABEL_UP, KEY_LABEL_DOWN, KEY_LABEL_LEFT, KEY_LABEL_RIGHT, KEY_LABEL_A, KEY_LABEL_B,
            KEY_LABEL_S1, KEY_LABEL_S2, KEY_LABEL_S3, KEY_LABEL_S4, KEY_LABEL_S5, KEY_LABEL_S6, KEY_LABEL_S7, KEY_LABEL_S8, KEY_LABEL_S9,KEY_LABEL_S10,KEY_LABEL_S11,KEY_LABEL_S12,KEY_LABEL_S13,KEY_LABEL_S14,KEY_LABEL_S15,
            KEY_LABEL_FUN1, KEY_LABEL_FUN2, KEY_LABEL_FUN3, KEY_LABEL_FUN4,KEY_LABEL_FUN5,KEY_LABEL_FUN6,KEY_LABEL_FUN7,
            KEY_LABEL_ROULETTE0,KEY_LABEL_ROULETTE1,KEY_LABEL_ROULETTE2,KEY_LABEL_ROULETTE3,KEY_LABEL_ROULETTE4,
            KEY_LABEL_LEFT_UP,KEY_LABEL_LEFT_DOWN,KEY_LABEL_RIGHT_UP,KEY_LABEL_RIGHT_DOWN
        };

        public static List<string> codeP1 = new List<string>() { "Up", "Down", "Left", "Right", "Fire1", "Jump",
            "Slot1", "Slot2", "Slot3", "Slot4", "Slot5", "Slot6", "Slot7", "Slot8", "Slot9","Slot10","Slot11","Slot12","Slot13","Slot14","Slot15",
            "Function1", "Function2", "Function3", "Function4","Function5","Function6","Function7",
            "Roulette0", "Roulette1", "Roulette2", "Roulette3","Roulette4",

            "Left_Up","Left_Down","Right_Up","Right_Down"
        };


        //public static List<string> codeP1_Joy = new List<string>() {   Gamesir.GamesirInput.AXIS_Y,  Gamesir.GamesirInput.AXIS_Y,  Gamesir.GamesirInput.AXIS_X,   Gamesir.GamesirInput.AXIS_X,  Gamesir.GamesirInput.BTN_B, "Jump", Gamesir.GamesirInput.BTN_L1, Gamesir.GamesirInput.BTN_R1, Gamesir.GamesirInput.BTN_L2,Gamesir.GamesirInput.BTN_R2,"Slot6", Gamesir.GamesirInput.BTN_Y, Gamesir.GamesirInput.BTN_X, Gamesir.GamesirInput.BTN_A, "Function3", "Function4",
        //     "Left_Up","Left_Down","Right_Up","Right_Down"
        //};
        //public static ArrayList codeP1 = new ArrayList(8) { "Up_2p", "Down_2p", "Left_2p", "Right_2p", "Fire1_2p", "Jump_2p", "Fire2_2p", "Fire3_2p" };
        public static List<string> codeP2 = new List<string>() { "Up_2p", "Down_2p", "Left_2p", "Right_2p", "Fire1_2p", "Jump_2p",
            "Slot1_2p", "Slot2_2p", "Slot3_2p", "Slot4_2p", "Slot5_2p", "Slot6_2p", "Slot7_2p", "Slot8_2p", "Slot9_2p","Slot10_2p","Slot11_2p","Slot12_2p","Slot13_2p","Slot14_2p", "Slot15_2p",
            "Function1_2P", "Function2_2P", "Function3_2P", "Function4_2P","Function5_2P","Function6_2P","Function7_2P",
            "Roulette0_2p", "Roulette1_2p", "Roulette2_2p", "Roulette3_2p","Roulette4_2p",
             "Left_Up_2p","Left_Down_2p","Right_Up_2p","Right_Down_2p"
        };

        static bool _bLock = false;  
        public static void Lock(bool bLock)
        {
            _bLock = bLock;
        }

        public static long ReadKeyState(List<string> codeArray, bool reverseDirection)
        {
            if (_bLock)
                return 0;

            // 状态
            long state = 0;
            // 左右状态
            long LRState = 0;
            for (int i = 0; i < codeArray.Count; i++)
            {
                //Debug.LogError(codeArray[i] + "reverseDirection---"+ reverseDirection+"---" + KeyState.IsDown(codeArray[i], reverseDirection));

                if (KeyState.IsDown(codeArray[i], reverseDirection) || VirtualKeyState.IsDown(codeArray[i], reverseDirection))
                {
                    if (i == 2 || i == 3)
                    {
                        LRState = LRState | KeyKtates[i];
                    }
                    else
                    {
                        state = state | KeyKtates[i];
                    }
                }
            }
            // 方向强锁向左
            if (VirtualKeyState.lockDirection == -1)
            {
                // right 位为0 left 位为1
                //LRState = LRState & 7 | 4;
                LRState = KEY_STATE_LEFT;
            }
            else if (VirtualKeyState.lockDirection == 1) // 方向强锁向右
            {
                // right 位为1  left 位为0
                //LRState = LRState & 11 | 8;
                LRState = KEY_STATE_RIGHT;
            }
            // 拼合位
            return (long)(state | LRState);
        }





        public static bool ReadSingleState(ref long keyState, int reverseIndex)
        {
            if (_bLock)
                return false;
            //超32了 改用long
            long long_keyState = keyState;
            // 位移 反向索引数 与1   判断keystate 的二进制 上第reverseIndex位 是否是1（开启）
            return (long_keyState >> reverseIndex & 1) == 1;
        }
        public static bool ReadSingleStateByKeyLable(ref long keyState, string keyLable)
        {
            int index = 0;
            bool has = reverseIndexMap.TryGetValue(keyLable, out index);
            // 字典里有值正常读取,否则false
            return has ? ReadSingleState(ref keyState, index) : false;
        }
    }
}
