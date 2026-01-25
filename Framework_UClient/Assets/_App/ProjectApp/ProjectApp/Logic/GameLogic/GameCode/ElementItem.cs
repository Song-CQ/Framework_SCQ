using System;
using System.Diagnostics.Tracing;
using System.Text;
using DG.Tweening;
using FutureCore;
using ProjectApp;
using ProjectApp.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlasticGui.PlasticTableColumn;
using static UnityEditor.LightingExplorerTableColumn;

namespace ProjectApp
{
    [Serializable]
    public struct ElementData
    {
        public int X;
        public int Y;
        public ElementType Type;
        public int data1;
        public int data2;
        public int data3;

        

        // Unity需要默认构造函数
        public ElementData(ElementType type = ElementType.Fixed_None)
        {
            X = 0;
            Y = 0;

            data1 = 0;
            data2 = 0;
            data3 = 0;
            
            Type = type;
        }
        public ElementData Set(ElementData data)
        {
            X = data.X;
            Y = data.Y;
            Type = data.Type;

            data1 = data.data1;
            data2 = data.data2;
            data3 = data.data3;
            return this;
        }


        public ElementData SetType(ElementType type)
        {
            Type = type;
            return this;
        }

        // 提供修改坐标的方法（返回新实例）
        public ElementData SetPot(int x, int y)
        {
            X = x;
            Y = y;
            return this;
        }


        // 实现 IEquatable
        public bool Equals(ElementData other)
        {
            return X == other.X && Y == other.Y && Type == other.Type;
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
        
        public override string ToString()
        {
            return string.Format("[{0},{1}]:{2}",X,Y,Type);
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

        public Vector3 Pos
        {
            get => pos;
            set 
            {
                pos = value; 
                Transform.localPosition = pos;
            }
        }

        public Collider Collider {get;private set;}

        private Vector3 pos;
        public bool isSelect;
        
        private bool _active = false;
        public bool Active 
        {
            get=> _active;
        }

        private Transform changeTrf;

        private SpriteRenderer[] changeIcons = new SpriteRenderer[3];

        public ElementItem() { }
        
        public ElementItem(Transform transform)
        {
            Transform = transform;
            Collider = Transform.GetComponent<BoxCollider>();
            icon = Transform.Find("icon").GetComponent<SpriteRenderer>();
            selectGo = Transform.Find("selectGo").gameObject;
            Transform.gameObject.AddComponent<DebugElementItem>().Init(this);

            changeTrf = Transform.Find("changeIcon");
            changeIcons[0] = changeTrf.GetChild(0).GetComponent<SpriteRenderer>();
            changeIcons[1] = changeTrf.GetChild(1).GetComponent<SpriteRenderer>();
            changeIcons[2] = changeTrf.GetChild(2).GetComponent<SpriteRenderer>();


            _active = Transform.gameObject.activeSelf;
            SetActive(true);
        }


        public void Init(ElementData _data)
        {

            SetData(_data);
            RefreshView();

        }

        public void SetData(ElementData _data)
        {
            Data = _data;
        }

        public void SetActive(bool value)
        {
            if (value != _active)
            {
                Transform.SetActive(value);
                _active = value;
            }

        }


        public void RefreshView()
        {
            if(ElementTypeTool.CheckType_HasIcon(Data.Type))
            { 
               icon.sprite = GameTool.GetSprite(Data.Type);
            }

            if (Data.Type == ElementType.Item_Change)
            {
                changeTrf.gameObject.SetActive(true);

                changeIcons[0].sprite = GameTool.GetSprite((ElementType)Data.data1);
                changeIcons[1].sprite = GameTool.GetSprite((ElementType)Data.data2);
                changeIcons[2].sprite = GameTool.GetSprite((ElementType)Data.data3);

            }
            else
            { 
                changeTrf.gameObject.SetActive(false);
            }

            selectGo.SetActive(isSelect);

        }

        public void Release()
        {
            SetSelect(false);
            SetActive(false);

        }

        public void SetSelect(bool v)
        {
            if(v == isSelect) return;
            isSelect = v;

            selectGo.SetActive(isSelect);

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



public class DebugElementItem : MonoBehaviour
{
    public ElementItem elementItem;

    [SerializeField]
    public ElementData Data;
    public bool isSelect;

    public ElementType newType;

    
    public System.Collections.Generic.List<string> InfoText = new System.Collections.Generic.List<string>();
    string oldname;

    [Button("设置新类型")]
    public void SetData()
    {
        Data.SetType(newType);
        elementItem.SetData(Data);

        GameTool.GameCore.Data.SetElementData(Data);
        elementItem.RefreshView();
    }

    public void Init(ElementItem _elementItem)
    {
        elementItem = _elementItem;
        oldname = string.Format("{0}_{1} : {2}", Data.X, Data.Y, Data.Type);
  
        InfoText.Add(oldname);
    }

    public void Update()
    {
        if (elementItem == null) return;

        Data = elementItem.Data;
        isSelect = elementItem.isSelect;
        //elementItem.selectGo.SetActive(isSelect);
        transform.name = string.Format("{0}_{1} : {2}", Data.X, Data.Y, Data.Type);
        if (transform.name != oldname)
        {
            oldname = transform.name;

            InfoText.Add(oldname);
        }


    }
}

