/****************************************************
    文件：GameMgr.cs
	作者：Clear
    日期：2022/4/16 22:46:13
    类型: 逻辑脚本
	功能：Nothing
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace ProjectApp
{
    public class GameRoodMgr : MonoBehaviour
    {
        public Transform SprMgr;

        private ItemData RoodItem;

        private List<ItemData> itemDataLst;

        public void StartLoad()
        {
            Debug.Log("开始加载");
            StartCoroutine(GameStart());
        }

        private IEnumerator GameStart()
        {
            itemDataLst = new List<ItemData>();

            RoodItem = new ItemData();
            RoodItem.trf = SprMgr.Find("RoodGame");
            RoodItem.pot = RoodItem.trf.localPosition;
            EventTrigger eventTrigger = RoodItem.trf.gameObject.AddComponent<EventTrigger>();
            RoodItem.trf.gameObject.AddComponent<BoxCollider>();
           
            EventTrigger.Entry Dure = new EventTrigger.Entry();
            Dure.eventID = EventTriggerType.Drag;
            Dure.callback.AddListener(OnDrag);
            eventTrigger.triggers.Add(Dure);
           
            EventTrigger.Entry BeginDrag = new EventTrigger.Entry();
            BeginDrag.eventID = EventTriggerType.BeginDrag;
            BeginDrag.callback.AddListener(OnBeginDrag);
            eventTrigger.triggers.Add(BeginDrag);

            EventTrigger.Entry EndDrag = new EventTrigger.Entry();
            EndDrag.eventID = EventTriggerType.EndDrag;
            EndDrag.callback.AddListener(OnEndDrag);
            eventTrigger.triggers.Add(EndDrag);


            var assetBundlePath = "file://"+Application.persistentDataPath + "/AssetBundle/imgab";
            var request = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath);

            yield return request;
            var myLoadedAssetBundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);

            Sprite[] sprites = myLoadedAssetBundle.LoadAllAssets<Sprite>();
            int Conts = 5;

            int ling = 1;

            int defX = 0;
            int defY = 0;

            int currY = 0;

            int add = 10;

            foreach (var item in sprites)
            {
                ItemData data = new ItemData();
                SpriteRenderer sprite = new GameObject().AddComponent<SpriteRenderer>();
                sprite.sprite = item;
                data.trf = sprite.transform;
                data.trf.SetParent(SprMgr);
                data.pot = new Vector2(defX, defY);
                itemDataLst.Add(data);

                sprite.drawMode = SpriteDrawMode.Sliced;
                sprite.size = Vector2.one * 10;

                currY++;
                if (currY >= Conts)
                {
                    defY = 0;
                    defX += (ling + add);
                    currY = 0;
                } else
                {
                    defY += (ling + add);
                }

            }
            isStart = true;

        }

        private Vector2 startPot;

        private void OnEndDrag(BaseEventData arg0)
        {
            PointerEventData pointerEventData = arg0 as PointerEventData;
            startPot = Vector2.zero;

        }

        private void OnBeginDrag(BaseEventData arg0)
        {
            PointerEventData pointerEventData = arg0 as PointerEventData;
            startPot = pointerEventData.position;
            Debug.Log("开始拖拽"+ startPot);
        }

        private void OnDrag(BaseEventData arg0)
        {
            PointerEventData pointerEventData = arg0 as PointerEventData;

            Vector2 pot = pointerEventData.position - startPot;
            RoodItem.trf.localPosition = (Vector2)RoodItem.trf.localPosition + pot * Time.deltaTime * MoveSeep;
            startPot = pointerEventData.position;
            Debug.Log("拖拽" + pointerEventData.position);
        }

        public void OnClick()
        {
            Debug.Log("点击");
        }

        private void Update()
        {
            if (!isStart)
            {
                return;
            }


            RoodItem.pot = RoodItem.trf.localPosition;
            foreach (var item in itemDataLst)
            {
                item.AddRepulsion(this, RoodItem);
                item.trf.localPosition = item.pot + item.repulsion;
                item.trf.localScale = item.size * Vector2.one;
            }


        }

        [Header("斥力的最大值")]
        public float MaxRand = 15;
        [Header("斥力放大倍数")]
        public float A3 = 10;
        [Header("最远生效距离")]
        public float A4 = 20;
        [Header("最大的缩小的值")]
        public float maxSize = 0.65f;

        
        public float MoveSeep = 10;
        private bool isStart =false;

        public class ItemData
        {
            public Transform trf;

            public Vector2 pot;

            public float size = 1;

            public Vector2 repulsion;

            public void AddRepulsion(GameRoodMgr gameRoodMgr, ItemData itemData)
            {
                repulsion = pot - itemData.pot;
                float val = gameRoodMgr.A4 - repulsion.magnitude;
                if (val <= 0)
                {
                    val = 0;
                }
                else
                {
                    val = val / gameRoodMgr.A4;
                }
                float rand = val * gameRoodMgr.A3;
                if (rand >= gameRoodMgr.MaxRand)
                {
                    rand = gameRoodMgr.MaxRand;
                }
                size = (1 - (1-gameRoodMgr.maxSize) * (rand / gameRoodMgr.MaxRand));

                repulsion = repulsion.normalized * rand;

            }

        }





    }
}