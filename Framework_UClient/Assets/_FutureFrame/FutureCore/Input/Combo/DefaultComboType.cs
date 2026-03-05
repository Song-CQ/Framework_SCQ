using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionGameLibrary
{
    public class DefaultComboType
    {
        public static string UP = "up(KD)=0-0";
        public static string DOWN = "down(KD)=0-0";
        public static string LEFT = "left(KD)=0-0";
        public static string RIGHT = "right(KD)=0-0";
        public static string UP_HOLD = "up(KH)=0-0";
        public static string DOWN_HOLD = "down(KH)=0-0";
        public static string LEFT_HOLD = "left(KH)=0-0";
        public static string RIGHT_HOLD = "right(KH)=0-0";
        public static string UP_UP = "up(KU)=0-0";
        public static string DOWN_UP = "down(KU)=0-0";
        public static string LEFT_UP = "left(KU)=0-0";
        public static string RIGHT_UP = "right(KU)=0-0";
        public static string A = "a(KD)=0-0";
        public static string A2 = "a(KD)=0-1";
        public static string B = "b(KD)=0-0";
        public static string A_DOWN = "down(KH)=0-0,a(KD)=0-0";
        public static string A_HOLD = "a(KH)=0-0";
        public static string B_HOLD = "b(KH)=0-0";
        public static string A_UP = "a(KU)=0-0";
        public static string B_UP = "b(KU)=0-0";
        public static string RUN_LEFT = "left(KD)=0-0,left(KU)=0-10,left(KD)=1-12";
        public static string RUN_RIGHT = "right(KD)=0-0,right(KU)=0-10,right(KD)=1-12";
        public static string BACK_JUMP = "down(KH)=0-0,b(KD)=0-0";
        public static string GO_DOWN = "down(KH)=0-0,b(KD)=0-0";

        public static string DASH_L = "left(KD)=0-0,left(KU)=0-10,left(KD)=1-12";
        public static string DASH_R = "right(KD)=0-0,right(KU)=0-10,right(KD)=1-12";

        public static string CHARGE_A = "a(KH)=0-10-10";

        public static string SKILL_A_01 = "s1(KD)=0-0";
        public static string SKILL_A_02 = "s2(KD)=0-0";
        public static string SKILL_A_03 = "s3(KD)=0-0";
        public static string SKILL_A_04 = "s4(KD)=0-0";
        public static string SKILL_A_05 = "s5(KD)=0-0";
        public static string SKILL_A_06 = "s6(KD)=0-0";
        public static string SKILL_A_07 = "s7(KD)=0-0";
        public static string SKILL_A_08 = "s8(KD)=0-0";
        public static string SKILL_A_09 = "s9(KD)=0-0";
        public static string SKILL_A_10 = "s10(KD)=0-0";
        public static string SKILL_A_11 = "s11(KD)=0-0";
        public static string SKILL_A_12 = "s12(KD)=0-0";
        public static string SKILL_A_13 = "s13(KD)=0-0";
        public static string SKILL_A_14 = "s14(KD)=0-0";
        public static string SKILL_A_15 = "s15(KD)=0-0";


        public static string SKILL_A_01_UP = "s1(KU)=0-0";
        public static string SKILL_A_02_UP = "s2(KU)=0-0";
        public static string SKILL_A_03_UP = "s3(KU)=0-0";
        public static string SKILL_A_04_UP = "s4(KU)=0-0";
        public static string SKILL_A_05_UP = "s5(KU)=0-0";
        public static string SKILL_A_06_UP = "s6(KU)=0-0";
        public static string SKILL_A_07_UP = "s7(KU)=0-0";
        public static string SKILL_A_08_UP = "s8(KU)=0-0";
        public static string SKILL_A_09_UP = "s9(KU)=0-0";
        public static string SKILL_A_10_UP = "s10(KU)=0-0";
        public static string SKILL_A_11_UP = "s11(KU)=0-0";
        public static string SKILL_A_12_UP = "s12(KU)=0-0";
        public static string SKILL_A_13_UP = "s13(KU)=0-0";
        public static string SKILL_A_14_UP = "s14(KU)=0-0";
        public static string SKILL_A_15_UP = "s15KU)=0-0";



        public static string SKILL_A_01_HOLD = "s1(KH)=0-0";
        public static string SKILL_A_02_HOLD = "s2(KH)=0-0";
        public static string SKILL_A_03_HOLD = "s3(KH)=0-0";
        public static string SKILL_A_04_HOLD = "s4(KH)=0-0";
        public static string SKILL_A_05_HOLD = "s5(KH)=0-0";
        public static string SKILL_A_06_HOLD = "s6(KH)=0-0";
        public static string SKILL_A_07_HOLD = "s7(KH)=0-0";
        public static string SKILL_A_08_HOLD = "s8(KH)=0-0";
        public static string SKILL_A_09_HOLD = "s9(KH)=0-0";
        public static string SKILL_A_10_HOLD = "s10(KH)=0-0";
        public static string SKILL_A_11_HOLD = "s11(KH)=0-0";
        public static string SKILL_A_12_HOLD = "s12(KH)=0-0";
        public static string SKILL_A_13_HOLD = "s13(KH)=0-0";
        public static string SKILL_A_14_HOLD = "s14(KH)=0-0";
        public static string SKILL_A_15_HOLD = "s15(KH)=0-0";


        public static string HOT_KEY = "hot(KD)=0-0";
        public static string HOT_KEY_UP = "hot(KU)=0-0";
        public static string HOT_KEY_HOLD = "hot(KH)=0-0";

        public static string FUNCTION_1 = "fun1(KD)=0-0";
        public static string FUNCTION_2 = "fun2(KD)=0-0";
        public static string FUNCTION_3 = "fun3(KD)=0-0";
        public static string FUNCTION_4 = "fun4(KD)=0-0";
        public static string FUNCTION_5 = "fun5(KD)=0-0";
        public static string FUNCTION_6 = "fun6(KD)=0-0";
        public static string FUNCTION_7 = "fun7(KD)=0-0";

        public static string S1 = "s1";
        public static string S2 = "s2";
        public static string S3 = "s3";
        public static string S4 = "s4";
        public static string S5 = "s5";
        public static string S6 = "s6";
        public static string S7 = "s7";
        public static string S8 = "s8";
        public static string S9 = "s9";
        public static string S10 = "s10";
        public static string S11 = "s11";
        public static string S12 = "s12";
        public static string S13 = "s13";
        public static string S14 = "s14";
        public static string S15 = "s15";

        public static string FUN1 = "fun1";
        public static string FUN2 = "fun2";
        public static string FUN3 = "fun3";
        public static string FUN4 = "fun4";
        public static string FUN5 = "fun5";
        public static string FUN6 = "fun6";


        public static string HOT = "hot";


        public static string LEFTUP = "left_up(KD)=0-0";
        public static string LEFTDOWN = "left_down(KD)=0-0";
        public static string LEFTUP_UP = "left_up(KU)=0-0";
        public static string LEFTDOWN_UP = "left_down(KU)=0-0";

        public static string RIGHTUP = "right_up(KD)=0-0";
        public static string RIGHTDOWN = "right_down(KD)=0-0";
        public static string RIGHTUP_UP = "right_up(KU)=0-0";
        public static string RIGHTDOWN_UP = "right_down(KU)=0-0";


        public static string RIGHTUP_HOLD = "right_up(KH)=0-0";
        public static string RIGHTDOWN_HOLD = "right_down(KH)=0-0";
        public static string LEFTUP_HOLD = "left_up(KH)=0-0";
        public static string LEFTDOWN_HOLD = "left_down(KH)=0-0";

        public static string ROULETTE_0 = "roulette0(KD)=0-0";
        public static string ROULETTE_1 = "roulette1(KD)=0-0";
        public static string ROULETTE_2 = "roulette2(KD)=0-0";
        public static string ROULETTE_3 = "roulette3(KD)=0-0";
        public static string ROULETTE_4 = "roulette4(KD)=0-0";

        

    }
}
