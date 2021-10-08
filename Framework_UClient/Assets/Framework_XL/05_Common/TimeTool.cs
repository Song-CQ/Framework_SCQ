using UnityEngine;
using UnityEditor;

namespace XL.Common
{

    public static class TimeTool
    {
        public enum TimeType
        {
            Figure = 0,
            Letter

        }
        public static string IntToString(int time)
        {
            string Time = string.Empty;

            Time = (time / 60).ToString() + ":";
            if (time % 60 < 10)
                Time += 0;
            Time += time % 60;



            return Time;
        }


    }
}