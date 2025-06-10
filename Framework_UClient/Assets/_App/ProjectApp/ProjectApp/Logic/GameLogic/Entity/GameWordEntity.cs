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
using TMPro;
using UnityEngine;

namespace ProjectApp
{

    public class GameWordEntity
    {
        public static Transform WordRood;
        public static Vector2 PictureRoodPot = Vector2.zero;

        #region Const

        private Transform AllPictureEntity;

        private GameObject PicturePlane;
        private Transform MeueTrf;



        private ObjectPool<PictureEntity> picturePool;
        private GObjectsPool entityPool;



        public static float p_w = 10;
        public static float p_h = 7;

        public float MaxDeltaPot = 3f;

        public float MeueItem_Size = 3;
        public float MeueItem_Interval = 1;

        public const string RoleAniPath = "Prefabs/GamePrefabs/RoleAni/";
        public const string PictureEntityPath = "Prefabs/GamePrefabs/PictureEntity";
        public const string DragEntityPath = "Prefabs/GamePrefabs/DragEntity";
        public const string RoleEntityPath = "Prefabs/GamePrefabs/RoleEntity";
        public const string SceneEntityPath = "Prefabs/GamePrefabs/SceneEntity";
        public const string TitleEntityPath = "Prefabs/GamePrefabs/TitleEntity";

        #endregion

        private Vector2 DragEntityPot;
        private MenuEntity SelectDragEntity;


        private List<MenuEntity> dragEntityMeue = new List<MenuEntity>();
        private List<PictureEntity> pictureEntityList = new List<PictureEntity>();

        private TextMeshPro titleText;
        private TextMeshPro resultText;


        private GameStructure gameStructure = null;
        private bool IsInit = false;

        public void Init()
        {

            WordRood = new GameObject("WordRood").transform;

            WordRood.transform.position = new Vector3(0, 0, 0);

            MeueTrf = new GameObject("MeueTrf").transform;
            MeueTrf.transform.SetParent(WordRood);
            MeueTrf.transform.localPosition = new Vector3(0, -8, 10);

            PicturePlane = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>("Prefabs/GamePrefabs/PicturePlane"));

            PicturePlane.transform.SetParent(WordRood.transform);
            PicturePlane.transform.localPosition = new Vector3(0f, 0f, 20f);

            GameObject TitleEntity = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>(TitleEntityPath));
            TitleEntity.transform.SetParent(WordRood.transform);
            TitleEntity.transform.localPosition = new Vector3(0f, 0f, 10f);
            titleText = TitleEntity.transform.Find("TitleText").GetComponent<TextMeshPro>();
            resultText = TitleEntity.transform.Find("ResultText").GetComponent<TextMeshPro>();



            AllPictureEntity = new GameObject("AllPictureEntity").transform;
            AllPictureEntity.transform.SetParent(WordRood.transform);
            AllPictureEntity.transform.localPosition = new Vector3(-9.5f, 3.2f, 17);

            if (picturePool == null)
            {
                picturePool = new ObjectPool<PictureEntity>(() =>
                {
                    PictureEntity entity = new PictureEntity();
                    entity.Init();
                    return entity;
                });
            }

            if (entityPool == null)
            {
                entityPool = new GObjectsPool();
                entityPool.InitRoot("EntityGoPool ", WordRood.transform);

            }

            if (SelectDragEntity == null)
            {
                SelectDragEntity = entityPool.Get<MenuEntity>(DragEntityPath);
                SelectDragEntity.gameObject.SetActive(false);
                SelectDragEntity.transform.SetParent(WordRood);
                SelectDragEntity.transform.localPosition = new Vector3(0, 0, 5);
            }


           

            IsInit = true;
            

        }
        private void AddListener()
        {
            gameStructure.gameSys.AddListener(GameEvent.GameFinish, GameFinish);
          

        }

        

        private void RemoveListener()
        {
            gameStructure.gameSys.RemoveListener(GameEvent.GameFinish, GameFinish);
        
        }

        private void GameFinish(object[] obj)
        {
            resultText.text = "关卡完成";
            resultText.color = Color.green;
            LogUtil.LogWarning("关卡完成");

        }



        public GameObject GetPrefabGo(string loadPath)
        {
            return entityPool.Get(loadPath);
        }

        public void ReleasePrefabGo(string loadPath, GameObject go)
        {
            entityPool.Release(loadPath, go);
        }

        public void ReleasePictureEntity(PictureEntity entity)
        {

            entity.Rest();
            picturePool.Release(entity);
        }
        public void ReleaseDragEntity(MenuEntity entity)
        {
            entity.RomveListener();
            entity.ResetEntity();
            entityPool.Release(DragEntityPath, entity);
        }

        public void BeginDragEntity(IDrag entity, Vector2 pot)
        {
            if (entity != null)
            {
                entity.BeginDrag();

                DragEntityPot = pot;

                pot = Camera.main.ScreenToWorldPoint(DragEntityPot);

                SelectDragEntity.transform.localPosition = new Vector3(pot.x, pot.y, 5);

                SelectDragEntity.SetData(entity.Data);
                SelectDragEntity.gameObject.SetActive(true);


            }

        }


        public void DragEntity(IDrag dragEntity, Vector2 position)
        {
            dragEntity.Drag();
            DragEntityPot = position;
        }
        public void EndDragEntity(IDrag dragEntity, Vector2 position)
        {
            dragEntity.EndDrag();
            RestSelectDragEntity();


            PictureEntity pictureEntity = GetPictureToScreenPoint(position);

            if (pictureEntity == null)
            {
                if (!(dragEntity is MenuEntity))
                {
                    if (dragEntity.Data.Type == DragEntityType.Event)
                    {
                        gameStructure.gameSys.Dispatch(GameEvent.RemoveEvent, dragEntity);
                    }
                    else if (dragEntity.Data.Type == DragEntityType.Role)
                    {

                        gameStructure.gameSys.Dispatch(GameEvent.RemoveRole, dragEntity);

                    }
                }

            }
            else
            {
                if (dragEntity.Data.Type == DragEntityType.Event)
                {
                    gameStructure.gameSys.Dispatch(GameEvent.SetEvent, pictureEntity, dragEntity);
                }
                else if (dragEntity.Data.Type == DragEntityType.Role)
                {
                    int rolePotIndex = -1;
                    if (pictureEntity.SceneEvent != null)
                    {
                        rolePotIndex = (pictureEntity.SceneEvent as SceneEventEntity).GetRolePotToScendPot(position);
                    }

                    gameStructure.gameSys.Dispatch(GameEvent.SetRole, pictureEntity, dragEntity, rolePotIndex);

                }
            }





        }

        private void RestSelectDragEntity()
        {
            SelectDragEntity.ResetEntity();
            SelectDragEntity.gameObject.SetActive(false);
            DragEntityPot = Vector2.zero;



        }

        public PictureEntity GetPictureToScreenPoint(Vector2 position)
        {
            Ray ray = CameraMgr.Instance.mainCamera.ScreenPointToRay(position);
            // 用于存储射线检测的结果
            RaycastHit hit;

            // 发射射线并检测碰撞
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Picture")))
            {
                Transform trf = hit.transform;

                foreach (var picture in pictureEntityList)
                {
                    if (picture.Transform == trf)
                    {


                        return picture;
                    }
                }


            }
            return null;
        }

        public void FillStructure(GameStructure _gameStructure)
        {
            gameStructure = _gameStructure;



            //标题开头
            titleText.text = _gameStructure.LevelData.title;

            resultText.text = "未完成";
            resultText.color = Color.red;



            for (int i = 0; i < gameStructure.LevelData.allPictureSum; i++)
            {
                PictureEntity picture = picturePool.Get();
                gameStructure.AddPicture(picture);
                pictureEntityList.Add(picture);

                picture.Transform.SetParent(AllPictureEntity);
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
            
            AddListener();
        }


        public void Updata()
        {
            if (!IsInit) return;

            if (SelectDragEntity.gameObject.activeInHierarchy)
            {
                float z = SelectDragEntity.transform.position.z;
                var pot = Camera.main.ScreenToWorldPoint(DragEntityPot);
                pot = Vector2.MoveTowards(SelectDragEntity.transform.position, pot, MaxDeltaPot);
                pot.z = z;

                SelectDragEntity.transform.position = pot;
            }


        }

        private void AddDragEntityToMeue(MenuEntity entity)
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
            int l_index = face == 0 ? face : face - 1;
            int r_index = face + 1;

            float interval = MeueItem_Interval;
            float size = MeueItem_Size;
            if (face == 1)
            {
                dragEntityMeue[cellIndex].transform.localPosition = Vector3.zero;
            }

            float staretPot = face == 0 ? (interval + size) / 2 : (interval + size);

            float l_Pot = -staretPot;
            float r_Pot = staretPot;

            while (true)
            {
                bool b = true;
                if (l_index >= 0)
                {
                    b = false;

                    dragEntityMeue[l_index].transform.localPosition = new Vector3(l_Pot, 0, 0);

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
        private void ClearEntity()
        {
            foreach (var item in pictureEntityList)
            {

                ReleasePictureEntity(item);
            }
            pictureEntityList.Clear();

            //销毁或者回收 to do
            foreach (var item in dragEntityMeue)
            {
                ReleaseDragEntity(item);
            }
            dragEntityMeue.Clear();
        }


        private MenuEntity GetDragEntity(DragEntityType entityType)
        {
            MenuEntity dragEntity = entityPool.Get<MenuEntity>(DragEntityPath);

            dragEntity.Init();
            dragEntity.AddListener();

            return dragEntity;
        }



        public void Rest()
        {

            gameStructure = null;


            ClearEntity();
            RemoveListener();

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