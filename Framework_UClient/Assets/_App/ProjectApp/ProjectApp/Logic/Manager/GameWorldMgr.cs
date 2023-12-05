/****************************************************
    文件: GameManager.cs
    作者: Clear
    日期: 2023/12/4 14:59:5
    类型: 逻辑脚本
    功能: 游戏世界管理器
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameWorldMgr : BaseMgr<GameWorldMgr>
    {
        private GameSys _gameSys;
        private  GameWord gameWord;


        protected override void New()
        {
            base.New();
            _gameSys = new GameSys();
        }
        public override void Init()
        {
            base.Init();
            gameWord = new GameWord();
            gameWord.Init();
        }

        public override void StartUp()
        {
            base.StartUp();

   
            gameWord.FillData(0);

        }
    }


    public class GameWord
    {
        public static Transform WordRood;
        public static Transform AllPictureEntity;
        public static Transform EntityPool;
        public static Vector2 PictureRoodPot = Vector2.zero;

        public LevelData levelData;

        public List<PictureEntity> pictureEntity; 

        private ObjectPool<PictureEntity> picturePool;

        public void Init()
        {
            WordRood = new GameObject("WordRood").transform;
            WordRood.transform.position = new Vector3(0, 0, 0);
            AllPictureEntity = new GameObject("AllPictureEntity").transform;
            AllPictureEntity.transform.SetParent(WordRood.transform);
            AllPictureEntity.transform.localPosition = new Vector3(-9.5f, 3.2f, 17);
            EntityPool = new GameObject("EntityPool").transform;
            EntityPool.transform.SetParent(WordRood.transform);
            EntityPool.transform.localPosition = new Vector3(200, 0, 0);

            levelData = null;
            pictureEntity = new List<PictureEntity>();
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
                var picture =  picturePool.Get();
                picture.Show(i);
                pictureEntity.Add(picture);
            }


        }

        public void Rest()
        {
            foreach (var item in pictureEntity)
            {
                item.Rest();
                picturePool.Release(item);
            }
            pictureEntity.Clear();



        }

    }

    /// <summary>
    /// 画面实体
    /// </summary>
    public class PictureEntity
    {
        public Transform Transform { get; set; }
        public IScene Scene { private set; get; }

        public List<IRole> Roles { private set; get; }

        public void SetScene(IScene newScene)
        {
            Scene?.Exit();

            Scene = newScene;
            Scene.Show();

        }

        public int GetNullRolePot()
        {
            for (int i = 0; i < Roles.Count; i++)
            {
                if (Roles[i] == null)
                {
                    return i;
                } 
            }
            return Roles.Count - 1;
        }

        public bool AddRole(IRole role,int index = -1)
        {
            if (Scene != null)
            {
                if (index == -1)
                {
                    index = GetNullRolePot();
                }

                Roles[index] = role;

            }
            return false;

        }
        public bool RemoveRole(int index)
        {
            if (Scene != null&&index>=0)
            {
                if (index < Roles.Count)
                {
                    if (Roles[index]!=null)
                    {
                        Roles[index] = null;

                        return true;
                    }
                  
                }
            }
            return false;
        }

        public void Init()
        {
            Transform = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>("Prefabs/GamePrefabs/PictureEntity")).transform;
            Roles = new List<IRole>();
        }
        public void Show(int index)
        {
            Transform.SetActive(true);
            Transform.SetParent(GameWord.AllPictureEntity);
            Vector2 pot = new Vector2(TestMgr.Instance.w * (index % 3), - TestMgr.Instance.h*(index/3));
            Transform.localPosition = GameWord.PictureRoodPot + pot;
        }
        public void Rest()
        {
            Scene = null;
            Roles.Clear();
            Transform.SetParent(GameWord.EntityPool);
            Transform.localPosition = Vector3.zero;
            Transform.SetActive(false);
        }
    }

    public class LevelData
    {
        public int allPictureSum = 6;


    }


    public interface IScene
    {
        public void Exit();
        public void Show();

    }


    public interface IRole
    {
        
    }
}