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
        public static Transform EntityPool;
        public static Vector2 PictureRoodPot = Vector2.zero;
        private static ObjectPool<PictureEntity> picturePool;

        public GameObject PicturePlane;
        private GameStructure gameStructure = null;


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

            if (picturePool != null)
            {
                picturePool = new ObjectPool<PictureEntity>(() => {
                    PictureEntity entity = new PictureEntity();
                    entity.Init();
                    return entity;
                });
            }

        }

       
        public static void ReleasePictureEntity(PictureEntity entity)
        {
            picturePool.Release(entity);
        }
              

        public void FillStructure(GameStructure _gameStructure)
        {
            gameStructure = _gameStructure;


            for (int i = 0; i < gameStructure.LevelData.allPictureSum; i++)
            {
                var picture = picturePool.Get();
                
                picture.Show(i);
                gameStructure.AddPicture(picture);          
            }
        }



        public void Rest()
        {
            gameStructure.Rest();
            gameStructure = null;


        }
        public void Dispose()
        {
            Rest();
            GameObject.Destroy(WordRood);
            WordRood = null;
            PicturePlane = null;
            AllPictureEntity = null;
            EntityPool = null;
            picturePool = null;
        }




    }


}