/****************************************************
    文件: GameTool.cs
    作者: Clear
    日期: 2026/1/17 22:44:57
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using log4net.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace ProjectApp
{
    public static class GameTool
    {
        public static EliminateGameCore GameCore;

        private static Dictionary<ElementType, Sprite> elementTypeSpr = new Dictionary<ElementType, Sprite>();
        public static Sprite GetSprite(ElementType type)
        {
            if (!elementTypeSpr.ContainsKey(type))
            {
                Sprite sprite = Resources.Load<Sprite>("ItemIcon/" + type.ToString());
                elementTypeSpr[type] = sprite;
            }
            return elementTypeSpr[type];
        }

        private static GameObject connection_Prefab;

        public static GameObject InstantiateConnectionPrefab()
        {
            if (connection_Prefab == null)
            {
                connection_Prefab = Resources.Load<GameObject>("Prefabs/GamePrefab/Connection");
            }

            return GameObject.Instantiate(connection_Prefab);
        }
        private static GameObject element_Prefab;

        public static GameObject InstantiateElementPrefab()
        {
            if (element_Prefab == null)
            {
                element_Prefab = Resources.Load<GameObject>("Prefabs/GamePrefab/Element");
            }

            return GameObject.Instantiate(element_Prefab);
        }
        public static ElementType[] AllBaseElements = new ElementType[] { ElementType.Item_A,ElementType.Item_B,ElementType.Item_C,ElementType.Item_D };

        public static void YatesElements()
        {
            for (int i = AllBaseElements.Length-1; i > 0; i--)
            {
                int j = RandomToInt(0, i+1);
                // 交换
                ElementType temp = AllBaseElements[i];
                AllBaseElements[i] = AllBaseElements[j];
                AllBaseElements[j] = temp;
            }

        }

        private static Random _systemRandom = new Random();
       
        /// <summary>
        /// 最大撤回步数
        /// </summary>
        public static int  maxUndoSum  = 10;

        public static void SetRandomSeed(int seed)
        {
            _systemRandom = new System.Random(seed);
            
        }

        public static int RandomToInt(int v1, int v2)
        {
           return _systemRandom.Next(v1,v2);
        }
        public static float RandomToFloat(float minInclusive, float maxInclusive)
        {
            return minInclusive + (float)_systemRandom.NextDouble() * (maxInclusive - minInclusive);
        }


        

        #region 斜对角生成
        const int MAX_ATTEMPTS_PER_CONNECTION = 100;

        

        // 随机选择斜对角方向

        public static Vector2Int GetDirection(Vector2Int start, Vector2Int end)
        {
            Vector2Int delta = end - start;

            // 判断是否为相邻位置
            if (Mathf.Abs(delta.x) <= 1 && Mathf.Abs(delta.y) <= 1)
            {
                // 4方向判断
                if (delta.x == 1 && delta.y == 0) return Vector2Int.right;
                if (delta.x == -1 && delta.y == 0) return Vector2Int.left;
                if (delta.x == 0 && delta.y == 1) return Vector2Int.up;
                if (delta.x == 0 && delta.y == -1) return Vector2Int.down;

                // 如果需要8方向，添加对角线判断
                if (delta.x == 1 && delta.y == 1) return new Vector2Int(1, 1);
                if (delta.x == -1 && delta.y == 1) return new Vector2Int(-1, 1);
                if (delta.x == 1 && delta.y == -1) return new Vector2Int(1, -1);
                if (delta.x == -1 && delta.y == -1) return new Vector2Int(-1, -1);
            }

            return Vector2Int.zero;
        }

        /// <summary>
        /// 将两个坐标 转化为 唯一的long key
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long EncodeConnection(ref Vector2Int a,ref Vector2Int b)
        {
            // 标准化顺序：确保a总是在b之前
            if (a.x > b.x || (a.x == b.x && a.y > b.y))
            {
                Vector2Int temp = a;
                a = b;
                b = temp;
            }

            // 使用位运算将4个short编码为一个long
            // 假设坐标范围在0-65535之间（够用了）
            // 格式：a.x(16位) | a.y(16位) | b.x(16位) | b.y(16位)
            long key = 0L;
            key |= ((long)a.x & 0xFFFF) << 48;
            key |= ((long)a.y & 0xFFFF) << 32;
            key |= ((long)b.x & 0xFFFF) << 16;
            key |= ((long)b.y & 0xFFFF);

            return key;
        }

        #endregion
    }
}