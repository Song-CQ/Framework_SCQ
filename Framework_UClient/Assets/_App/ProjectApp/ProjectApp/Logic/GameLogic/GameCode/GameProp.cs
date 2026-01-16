/****************************************************
    文件: GameProp.cs
    作者: Clear
    日期: 2026/1/16 21:20:41
    类型: 逻辑脚本
    功能: 道具类
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    // 道具类型枚举
    public enum PropType
    {
        Horizontal, // 横向消除
        Vertical,   // 竖向消除
        Bomb,       // 炸弹
        Wild        // Wild道具
    }
    public class GameProp : MonoBehaviour
    {
        public System.Action<PropType, int, int> OnPropClicked;

        private PropType propType;
        private int gridX;
        private int gridY;

        public void Initialize(PropType type, int x, int y)
        {
            propType = type;
            gridX = x;
            gridY = y;
        }

        void OnMouseDown()
        {
            OnPropClicked?.Invoke(propType, gridX, gridY);
        }
    }
}