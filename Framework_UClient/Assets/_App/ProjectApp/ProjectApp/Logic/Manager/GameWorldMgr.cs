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
using ProjectApp.Data;

namespace ProjectApp
{
    public class GameWorldMgr : BaseMonoMgr<GameWorldMgr> 
    {
        /// <summary>
        /// 游戏表现实体 和 数据实体通过 gamesys 交互
        /// </summary>
        private GameSys gameSys;
        
        /// <summary>
        /// 游戏表现实体
        /// </summary>
        public GameWordEntity GameEntity { private set; get; }


        /// <summary>
        /// 游戏逻辑实体
        /// </summary>
        public GameStructure GameStructure { private set; get; }
       

        protected override void New()
        {
            base.New();
            gameSys = new GameSys();
        }
        public override void Init()
        {
            base.Init();

            gameSys.Init();

            GameEntity = new GameWordEntity();
            GameEntity.Init();
            GameObject.DontDestroyOnLoad(GameWordEntity.WordRood);               



            

        }


        public override void StartUp()
        {
            base.StartUp();
            
            GameStructure = GetGameStructure();

        }

        public void StartGame()
        {
            LevelData levelData = GetLevelData(1); 


            GameStructure.FillData(levelData);
            GameEntity.FillStructure(GameStructure);
            Debug.LogWarning("开始游戏");


        }

        private void FixedUpdate()
        {
            if (GameEntity!=null)
            {
                GameEntity.Updata();
            }
        }

        private GameStructure GetGameStructure()
        {
            GameStructure gameStructure = new GameStructure();

            gameStructure.gameSys = gameSys;
            return gameStructure;
        }

        private LevelData GetLevelData(int id)
        {
            var VO = ConfigDataMgr.Instance.GetConfigVO<LevelDataVO>(ConfigVO.LevelData, id);

            LevelData levelData = new LevelData(VO);

            foreach (var roleId in levelData.data.roles)
            {
                Role_Data role_Data = new Role_Data(roleId);

                levelData.roles.Add(role_Data);

            }

            foreach (var roleId in levelData.data.events)
            {
                Event_Data event_Data = new Event_Data(roleId);

                levelData.events.Add(event_Data);

            }


            return levelData;
        }



    }

    
    
}                                                                                                                                                                                                                                                                                                                                           