using UnityEngine;
using System.Collections;

namespace ActionGameLibrary
{
    public class VirtualComboInput : ComboInput
    {
        public VirtualComboInput(bool exclusionMode)
            : base(exclusionMode)
        {
        }
        public VirtualComboInput()
            : base()
        {
        }

        public override void UpdateJoy()
        {
            _joyAngle = VirtualKeyState.joyAngle;
            _joyPower = VirtualKeyState.joyPower;
        }

        public override bool KeyIsDown(string key)
        {
            //强制关闭了输入
            if (!enable || netLock) return false;

            return VirtualKeyState.IsDown(key, reverseDirection);
        }
    }
}
