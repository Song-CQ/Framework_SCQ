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
    }
}