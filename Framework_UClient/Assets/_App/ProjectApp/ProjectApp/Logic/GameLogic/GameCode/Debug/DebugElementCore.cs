/****************************************************
    文件: DebugElementItem.cs
    作者: Clear
    日期: 2026/1/23 21:17:40
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using ILRuntime.Mono.Cecil.Cil;
using UnityEngine.UI;
using TMPro;

namespace ProjectApp
{

    [Serializable]
    public class Serializable2DArray<T>
    {
        [SerializeField]
        private List<T> data = new List<T>();

        [SerializeField]
        private int width = 5;
        [SerializeField]
        private int height = 5;

        public int Width => width;
        public int Height => height;

        public T this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    return default;

                int index = y * width + x;
                if (index >= 0 && index < data.Count)
                    return data[index];
                return default;
            }
            set
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    return;

                int index = y * width + x;
                if (index < data.Count)
                    data[index] = value;
            }
        }

        public void Resize(int newWidth, int newHeight)
        {
            width = Mathf.Max(1, newWidth);
            height = Mathf.Max(1, newHeight);

            // 调整数据大小
            List<T> newData = new List<T>(width * height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int oldIndex = y * width + x;
                    if (oldIndex < data.Count)
                        newData.Add(data[oldIndex]);
                    else
                        newData.Add(default);
                }
            }
            data = newData;
        }
    }

    public class DebugElementCore : MonoBehaviour
    {
        public EliminateGameCore core;
        public ElementGameData data;

        public TextMeshProUGUI text;


        public Serializable2DArray<ElementData> elementGrid = new Serializable2DArray<ElementData>();

        [Button("Init")]
        public void Start()
        {
            core = GetComponent<EliminateGameCore>();
            data = core.Data;

            SetValidate();


        }

        private void Update()
        {
            if (core==null)
            {
                return;
            }
            if (core.Data == null) return;
            foreach (var item in core.BoardData)
            {
                elementGrid[item.X,item.Y] = item;
            }

            text.text = core.Data.currentScore.ToString();



        }

        void SetValidate()
        {
            // 自动调整网格大小
            if (elementGrid.Width != core.Data.BoardWidth || elementGrid.Height != core.Data.BoardHeight)
            {
                elementGrid.Resize(core.Data.BoardWidth, core.Data.BoardHeight);
                UpdateGridCoordinates();
            }
        }

        void UpdateGridCoordinates()
        {
            for (int y = 0; y < elementGrid.Height; y++)
            {
                for (int x = 0; x < elementGrid.Width; x++)
                {
                    var element = elementGrid[x, y];
                    element.X = x;
                    element.Y = y;
                    elementGrid[x, y] = element;
                }
            }
        }


    }
}