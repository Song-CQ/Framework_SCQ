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
            LevelData levelData = GetLevelData(); 


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

        private LevelData GetLevelData()
        {
            LevelData levelData = new LevelData();

            levelData.title = "让伏羲和夏娃结婚";

            Role_Data role_Data_1 = new Role_Data();
            role_Data_1.Key = RoleKey.Fuxi;
            role_Data_1.allLabelKey.Add(LabelKey.CanLove);

            Role_Data role_Data_2 = new Role_Data();
            role_Data_2.Key = RoleKey.Nuwa;
            role_Data_2.allLabelKey.Add(LabelKey.CanLove);

            levelData.roles.Add(role_Data_1);
            levelData.roles.Add(role_Data_2);

            Event_Data event_Data1 = new Event_Data( EventKey.Love);



            Event_Data event_Data2 = new Event_Data(EventKey.Love);

            levelData.events.Add(event_Data1);
            //levelData.events.Add(event_Data2);


            return levelData;
        }



    }

    
    
}                                                                                                                                                                                                                                                                                                                                           