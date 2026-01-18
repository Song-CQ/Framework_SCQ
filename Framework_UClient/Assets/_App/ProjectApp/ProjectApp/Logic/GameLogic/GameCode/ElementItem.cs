using System;
using UnityEditor;
using UnityEngine;
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
        public void WithPosition(int x, int y)
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
            
        }


        public void Init(ElementData _data)
        {
            Data = _data;

            AddListener();

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

        public void Refresh()
        {
            icon.sprite = GameTool.GetSprite(Data.Type);


        }

        public void UpdatePosition(int x, int y)
        {
            Data.WithPosition(x, y);
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

        void OnMouseDown()
        {
            GameTool.GameCore.Dispatch(GameMsg.ClickElement,this);
        }

        public void Release()
        {
            RemoveListener();
        }
    }
}
