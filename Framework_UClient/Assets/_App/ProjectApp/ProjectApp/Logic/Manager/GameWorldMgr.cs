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
        private  GameWordEntity gameWord;

        private GameStructure gameStructure;

        protected override void New()
        {
            base.New();
            _gameSys = new GameSys();
        }
        public override void Init()
        {
            base.Init();
            gameWord = new GameWordEntity();
            gameWord.Init();
        }

        public override void StartUp()
        {
            base.StartUp();

            LevelData levelData = new LevelData();

            gameStructure = GetGameStructure();
            gameStructure.FillData(levelData);
            gameWord.FillStructure(gameStructure);

        }

        private GameStructure GetGameStructure()
        {
            GameStructure gameStructure = new GameStructure();


            return gameStructure;
        }



        public bool CanRoleSetSceneEvent(IRole role, BaseSceneEvent sceneEvent)
        {
            if (gameStructure == null)
            {
                return false;
            }
            return gameStructure.CanRoleSetSceneEvent(role, sceneEvent);
        }

        public bool GetHaveSceneKeyToIndex(string sceneEventKey, int index)
        {
            if (gameStructure == null)
            {
                return false;
            }
            return gameStructure.GetHaveSceneKeyToIndex(sceneEventKey, index);
        }
    }


    public class LevelData
    {
        public int allPictureSum = 6;
        

    }

    public class RoleData
    {
        public string key;

        /// <summary>
        /// 该角色经历过的场景
        /// </summary>
        public List<ISceneEvent> allSceneEvent;

    }

    public class GameStructure
    {
        /// <summary>
        /// 画面
        /// </summary>
        public List<BasePicture> AllPictures = new List<BasePicture>();


        public LevelData LevelData { get; private set; }

        private Dictionary<string,RoleData> roleDatas = new Dictionary<string, RoleData>();

        public void FillData(LevelData levelData)
        {
            AllPictures.Clear();

            LevelData = levelData;

        }

        public void AddPicture(BasePicture _picture)
        {
            AllPictures.Add(_picture);
        }

        public RoleData GetRoleData(string roleKey)
        {
            if (roleDatas.ContainsKey(roleKey))
            {
                return roleDatas[roleKey];
            }
            return null;
        }

        public void Rest()
        {
            LevelData = null;

            foreach (var _Pictures in AllPictures)
            {
                _Pictures.Rest();
            }
            AllPictures.Clear();

        }

       

        public bool CanRoleSetSceneEvent(IRole role, BaseSceneEvent sceneEvent)
        {


            return false;
        }

        public bool GetHaveSceneKeyToIndex(string sceneEventKey, int index)
        {
           

            if (true)
            {

            }
            return false;
        }
    }


    
}