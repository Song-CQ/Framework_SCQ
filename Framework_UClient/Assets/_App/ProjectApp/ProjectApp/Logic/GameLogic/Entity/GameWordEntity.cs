/****************************************************
    文件: GameWord.cs
    作者: Clear
    日期: 2023/12/6 11:32:43
    类型: 逻辑脚本
    功能: 游戏世界实体
*****************************************************/
using FutureCore;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameWordEntity
    {
        public static Transform WordRood;
        public static Transform AllPictureEntity;
        public static Transform EntityPool;
        public static Vector2 PictureRoodPot = Vector2.zero;

        public GameObject PicturePlane;
        public LevelData levelData;
        /// <summary>
        /// 所有的角色画面
        /// </summary>
        public List<PictureEntity> pictureEntityList;

        private ObjectPool<PictureEntity> picturePool;

        public void Init()
        {
            WordRood = new GameObject("WordRood").transform;
            WordRood.transform.position = new Vector3(0, 0, 0);

            PicturePlane = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>("Prefabs/GamePrefabs/PicturePlane"));

            PicturePlane.transform.SetParent(WordRood.transform);
            PicturePlane.transform.localPosition = new Vector3(0f, 0f, 20f);

            AllPictureEntity = new GameObject("AllPictureEntity").transform;
            AllPictureEntity.transform.SetParent(WordRood.transform);
            AllPictureEntity.transform.localPosition = new Vector3(-9.5f, 3.2f, 17);
            EntityPool = new GameObject("EntityPool").transform;
            EntityPool.transform.SetParent(WordRood.transform);
            EntityPool.transform.localPosition = new Vector3(200, 0, 0);

            levelData = null;
            pictureEntityList = new List<PictureEntity>();
            picturePool = new ObjectPool<PictureEntity>(() => {
                PictureEntity entity = new PictureEntity();
                entity.Init();
                return entity;
            });
        }

        public void FillData(int levenId)
        {
            levelData = new LevelData();

            for (int i = 0; i < levelData.allPictureSum; i++)
            {
                var picture = picturePool.Get();
                picture.Show(i);
                pictureEntityList.Add(picture);
            }


        }

        public void Rest()
        {
            foreach (var item in pictureEntityList)
            {
                item.Rest();
                picturePool.Release(item);
            }
            pictureEntityList.Clear();



        }

    }


    public class wordData
    {
        
    }
}