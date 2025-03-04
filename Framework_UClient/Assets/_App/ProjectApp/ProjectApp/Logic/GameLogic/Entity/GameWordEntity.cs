/****************************************************
    文件: GameWord.cs
    作者: Clear
    日期: 2023/12/6 11:32:43
    类型: 逻辑脚本
    功能: 游戏世界实体
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{

    public class GameWordEntity
    {
        public static Transform WordRood;
        

        public static Transform AllPictureEntity;
        public static Vector2 PictureRoodPot = Vector2.zero;

        private static ObjectPool<PictureEntity> picturePool;
        private static GObjectsPool entityPool;

        public static float p_w  =10;
        public static float p_h = 7;

        public float MeueItem_Size = 3;
        public float MeueItem_Interval = 1;

        public GameObject PicturePlane;
        private GameStructure gameStructure = null;
        private Transform MeueTrf;

        public DragEntity SelectDragEntity {private set; get;}
        public Vector2 DragEntityPot { set; get;}

        private List<DragEntity> dragEntityMeue = new List<DragEntity>();

        private const string dragEntityPath = "Prefabs/GamePrefabs/DragEntity";

        

        public void Init()
        {
            
            
            WordRood = new GameObject("WordRood").transform;
          
            WordRood.transform.position = new Vector3(0, 0, 0);
            
            MeueTrf = new GameObject("MeueTrf").transform;
            MeueTrf.transform.SetParent(WordRood);
            MeueTrf.transform.localPosition = new Vector3(0,-8,10);

            PicturePlane = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>("Prefabs/GamePrefabs/PicturePlane"));

            PicturePlane.transform.SetParent(WordRood.transform);
            PicturePlane.transform.localPosition = new Vector3(0f, 0f, 20f);


            AllPictureEntity = new GameObject("AllPictureEntity").transform;
            AllPictureEntity.transform.SetParent(WordRood.transform);
            AllPictureEntity.transform.localPosition = new Vector3(-9.5f, 3.2f, 17);
         
            if (picturePool == null)
            {
                picturePool = new ObjectPool<PictureEntity>(() => {
                    PictureEntity entity = new PictureEntity();
                    entity.Init();
                    return entity;
                });
            }

            if (entityPool == null)
            {
                entityPool = new GObjectsPool();
                entityPool.InitRoot("EntityPool ", WordRood.transform);
               
            }

        }

       
        public static void ReleasePictureEntity(PictureEntity entity)
        {
            picturePool.Release(entity);
        }  
        public static void ReleaseDragEntity(DragEntity entity)
        {
            entity.ResetEntity();
            entityPool.Release(dragEntityPath, entity);
        }
              

        public void FillStructure(GameStructure _gameStructure)
        {
            gameStructure = _gameStructure;


            for (int i = 0; i < gameStructure.LevelData.allPictureSum; i++)
            {
                var picture = picturePool.Get();
           
                gameStructure.AddPicture(picture);          
                picture.Show(i);
            }

            //创建事件按钮

            foreach (var item in gameStructure.LevelData.events)
            {
                item.Type = DragEntityType.Event;
                var dragItem = GetDragEntity(item.Type);
                dragItem.SetData(item);
                AddDragEntityToMeue(dragItem);
            }
            //创建角色按钮
            foreach (var item in gameStructure.LevelData.roles)
            {
                item.Type = DragEntityType.Role;

                var dragItem = GetDragEntity(item.Type);
                dragItem.SetData(item);
                AddDragEntityToMeue(dragItem);
            }

            SortMeueItem();

        }


        public void SetBingDragEntity(DragEntity entity)
        {
            if (entity)
            {
                SelectDragEntity.gameObject.SetActive(true);
                SelectDragEntity.SetData(entity.data);
                DragEntityPot = entity.transform.position;

            }
            else
            {
                SelectDragEntity.ResetEntity();
                SelectDragEntity.gameObject.SetActive(false);
                DragEntityPot = Vector2.zero;
            }
   
          

        }

        public void Updata()
        {
            
        }

        private void AddDragEntityToMeue(DragEntity entity)
        {
            dragEntityMeue.Add(entity);
            //添加进按钮啥的
            entity.transform.SetParent(MeueTrf);
            entity.transform.position = Vector3.zero;

          
        }

        

        public void SortMeueItem()
        {
            int cont = dragEntityMeue.Count;
            int cellIndex = cont / 2;
            if (cellIndex == 0) return;


            int face = cont % 2;
            int l_index = face ==0 ? face : face - 1;
            int r_index = face + 1;
            
            float interval = MeueItem_Interval;
            float size = MeueItem_Size;
            if (face == 1)
            {
                dragEntityMeue[cellIndex].transform.localPosition = Vector3.zero;
            }

            float staretPot = face == 0 ? (interval+size) / 2 : (interval + size);

            float l_Pot  =  - staretPot;
            float r_Pot  = staretPot;

            while (true)
            {
                bool b = true;
                if (l_index >= 0)
                {
                    b = false;

                    dragEntityMeue[l_index].transform.localPosition = new Vector3(l_Pot, 0,0);

                    l_Pot -= (interval + size);
                    l_index--;
                }
                if (r_index < cont)
                {
                    b = false;

                    dragEntityMeue[r_index].transform.localPosition = new Vector3(r_Pot, 0, 0);

                    r_Pot += (interval + size);

                    r_index++;
                }

                if (b) break;

            }

        }

        /// <summary>
        /// 清空拖拽实体
        /// </summary>
        private void ClearDragEntityMeue()
        {

            //销毁或者回收 to do
            foreach (var item in dragEntityMeue)
            {
                ReleaseDragEntity(item);
            }
            dragEntityMeue.Clear();
        }


        private DragEntity GetDragEntity(DragEntityType entityType)
        {
            DragEntity dragEntity = entityPool.Get<DragEntity>(dragEntityPath);

            //dragEntity.SetData();

            return dragEntity;
        }

     

        public void Rest()
        {
            gameStructure.Rest();
            gameStructure = null;

            ClearDragEntityMeue();
        }
        public void Dispose()
        {
            Rest();
           
            WordRood = null;
            PicturePlane = null;
            AllPictureEntity = null;
            picturePool.Dispose();
            picturePool = null;

            entityPool.Dispose();
            entityPool = null;

            GameObject.Destroy(WordRood);
        }




    }


}