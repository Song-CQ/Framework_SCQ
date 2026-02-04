/****************************************************
    文件: GameProp.cs
    作者: Clear
    日期: 2026/1/16 21:20:41
    类型: 逻辑脚本
    功能: 外置道具类
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    public enum ExternalProp
    {
        Hammer = 10000,
        Swipe,
        Horizontal,
        Vertical,
        AllRandom,
        Undo,
        Wild,
        AddScore,

    }

    public class ExternalPropItem : IRaycast3D
    {
        public Transform Transform;
        private SpriteRenderer icon;

        public Collider Collider {get;private set;}
        public ExternalProp type;


        public ExternalPropItem()
        {
            
        }

        public ExternalPropItem(GameObject go)
        {
            Transform = go.transform;
            Collider = go.AddComponent<BoxCollider>();
            icon = Transform.Find("icon").GetComponent<SpriteRenderer>();

        }

        public void SetType(ExternalProp _type)
        {
            
            type = _type;
            RefreshView();
        }

        public void RefreshView()
        {
            icon.sprite = GameTool.GetSprite(type);

           
        }

        public void Raycast_OnClick(Vector3 hitPoint)
        {
            GameTool.GameCore.ClickExternalPropItem(this);
        }

        public void Raycast_OnSwipe(Vector3 startPoint, Vector3 endPoint, IRaycast3D endIRaycast3D)
        {
            GameTool.GameCore.AddSwipeVector2(endPoint - startPoint);
        }

        

        






    }
}