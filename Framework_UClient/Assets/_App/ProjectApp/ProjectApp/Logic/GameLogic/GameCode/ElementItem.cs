using System;
using System.Diagnostics.Tracing;
using FutureCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlasticGui.PlasticTableColumn;

namespace ProjectApp
{

    public struct ElementData
    {
        public int X;
        public int Y;
        public ElementType Type;

        public ElementData(int x, int y, ElementType type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        // 提供修改坐标的方法（返回新实例）
        public void SetPot(int x, int y)
        {
            X = x;
            Y = y;
        }


        // 实现 IEquatable
        public bool Equals(ElementData other)
        {
            return X == other.X && Y == other.Y && Type == other.Type;
        }

        public void SetType(ElementType type)
        {
            Type = type;
        }

        // 重载运算符
        public static bool operator ==(ElementData left, ElementData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ElementData left, ElementData right)
        {
            return !left.Equals(right);
        }
    }

    //元素类
    public class ElementItem
    {
        public Transform Transform { get; private set; }

        public ElementData Data { get; private set; }
        
        private BoxCollider2D collide;
        private SpriteRenderer icon;

        private bool isHighlight;

        public Vector2 Pos
        {
            get => pos;
            set 
            {
                pos = value; 
                Transform.localPosition = pos;
            }
        }

        private Vector2 pos;

        public ElementItem() { }
        
        public ElementItem(Transform transform)
        {
            Transform = transform;
            collide = Transform.GetComponent<BoxCollider2D>();
            icon = Transform.Find("icon").GetComponent<SpriteRenderer>();
            
            // listener  = UIEventListener.GetEventListener(Transform);
            // listener.PointerClick_Event += OnClickEvent;
        }
        private void PointerClick_Event() 
        {
            GameTool.GameCore.Dispatch(GameMsg.ClickElement,this);
        }


        public void Init(ElementData _data)
        {
            Data = _data;

            AddListener();

            RefreshView();

        }

        private void AddListener()
        {
            GameTool.GameCore.AddListener(GameMsg.SelectElement, SelectElement);
            GameTool.GameCore.AddListener(GameMsg.DeselectElement, DeselectElement);
        }
        private void RemoveListener()
        {
            GameTool.GameCore.RemoveListener(GameMsg.SelectElement, SelectElement);
            GameTool.GameCore.RemoveListener(GameMsg.DeselectElement, DeselectElement);
        }

        public void RefreshView()
        {
            if(ElementTypeTool.CheckType_HasIcon(Data.Type))
            { 
               icon.sprite = GameTool.GetSprite(Data.Type);
            }
        }

        /// <summary>
        /// 选中元素
        /// </summary>
        void SelectElement(object o)
        {
            ElementItem element = (ElementItem)o;

            if (element.Data == Data)
            {
                icon.color =  Color.yellow;
            }

        }

        /// <summary>
        /// 取消选中元素
        /// </summary>
        void DeselectElement(object o)
        {
            ElementItem element = (ElementItem)o;

            if (element.Data == Data)
            {
                icon.color = Color.yellow;
            }

        }


        public void Release()
        {
            RemoveListener();

            SetHighlight(false);

        }

        public void SetHighlight(bool v)
        {
            if(v == isHighlight) return;
            isHighlight = v;
            if(isHighlight)
            {
                icon.color = Color.yellow;
            }else
            {
                icon.color = Color.white;
            }


        }
    }
}
