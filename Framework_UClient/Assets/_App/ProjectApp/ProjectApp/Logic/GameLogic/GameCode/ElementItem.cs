using System;
using System.Diagnostics.Tracing;
using DG.Tweening;
using FutureCore;
using ProjectApp.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlasticGui.PlasticTableColumn;

namespace ProjectApp
{
    [Serializable]
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
        
        public string ToString()
        {
            return string.Format("[{0},{1}]:{2}",X,Y,Type);
        }
    
    }

    public class DebugElementItem : MonoBehaviour
    {
        public ElementItem elementItem;
        
        [SerializeField]
        public ElementData Data; 
        public bool isSelect; 

        public void Init(ElementItem _elementItem)
        {
           elementItem =_elementItem;
        }

        public void Update()
        {
            if(elementItem==null)return;

            Data = elementItem.Data;
            isSelect = elementItem.isSelect;
            elementItem.selectGo.SetActive(isSelect);

        }
    }

    //元素类
    [Serializable]
    public class ElementItem:IRaycast3D_OnClick
    {
        public Transform Transform { get; private set; }

        public ElementData Data { get; private set; }
        
        private SpriteRenderer icon;
        public GameObject selectGo;

        public Vector2 Pos
        {
            get => pos;
            set 
            {
                pos = value; 
                Transform.localPosition = pos;
            }
        }

        public Collider Collider {get;private set;}

        private Vector2 pos;
        public bool isSelect;

        public ElementItem() { }
        
        public ElementItem(Transform transform)
        {
            Transform = transform;
            Collider = Transform.GetComponent<BoxCollider>();
            icon = Transform.Find("icon").GetComponent<SpriteRenderer>();
            selectGo = Transform.Find("selectGo").gameObject;
            Transform.gameObject.AddComponent<DebugElementItem>().Init(this);

        }


        public void Init(ElementData _data)
        {
            Data = _data;

            RefreshView();

        }


        public void RefreshView()
        {
            if(ElementTypeTool.CheckType_HasIcon(Data.Type))
            { 
               icon.sprite = GameTool.GetSprite(Data.Type);
            }

            selectGo.SetActive(isSelect);

        }

        public void Release()
        {
            isSelect = false;

        }

        public void SetSelect(bool v)
        {
            if(v == isSelect) return;
            isSelect = v;

            // selectGo.SetActive(isSelect);

        }

        public void Raycast_OnClick(Vector3 hitPoint)
        {
            GameTool.GameCore.Dispatch(GameMsg.ClickElement,this.Data);
        }


        public void StopAllDOTween()
        {
            


            Transform.localScale = Vector3.one;
            Transform.rotation = Quaternion.identity;
            pos = Vector3.zero;

        }
    }
}
