using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Endogine.Text
{
    public class Keyboard
    {
        //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput.asp
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern uint GetOEMCP();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetKeyNameTextW(uint lParam, System.Text.StringBuilder lpString, int nSize);
        //public static extern int GetKeyNameText(long lParam, string lpString, int nSize); //int GetKeyNameText(LONG lParam, LPTSTR lpString, int nSize)

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint MapVirtualKeyW(uint uCode, uint uMapType);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wparam, ref TVHITTESTINFO lparam);

        public Keyboard()
        {
            //uint cp = GetOEMCP();

            Dictionary<int, string> codeToName = new Dictionary<int, string>();
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 512; i++)
            {
                uint key = MapVirtualKeyW((uint)i, 0) << 16; //e.KeyCode
                int ret = GetKeyNameTextW(key, sb, 260);
                if (!codeToName.ContainsKey((int)key))
                    codeToName.Add((int)key, sb.ToString());
            }

            sb.Append(10);
            string s = sb.ToString();
        }
    }
}
