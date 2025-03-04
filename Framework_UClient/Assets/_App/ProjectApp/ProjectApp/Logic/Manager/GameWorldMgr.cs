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
        private GameSys gameSys;
        public GameWordEntity GameEntity { private set; get; }

        public GameStructure GameStructure { private set; get; }

        

        public class GameDispatcher : BaseDispatcher<GameDispatcher, GameEvent, object[]> { }

        public GameDispatcher gameDispatcher { private set; get; }

        protected override void New()
        {
            base.New();
            gameSys = new GameSys();
        }
        public override void Init()
        {
            base.Init();
            GameEntity = new GameWordEntity();
            GameEntity.Init();
            GameObject.DontDestroyOnLoad(GameWordEntity.WordRood);

            gameDispatcher = new GameDispatcher();

            gameDispatcher.AddListener(GameEvent.SetEvent, PlayerInput_SetEvent);         
            gameDispatcher.AddListener(GameEvent.SetRole, PlayerInput_SetRole);            
            gameDispatcher.AddListener(GameEvent.RemoveEvent, PlayerInput_RemoveEvent);   
            gameDispatcher.AddListener(GameEvent.RemoveRole, PlayerInput_RemoveRole);     

        }

        

       

        

    

        public override void StartUp()
        {
            base.StartUp();

            

        }

        public void StartGame()
        {
            LevelData levelData = GetLevelData(); 

            GameStructure = GetGameStructure();
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

            Role_Data role_Data_1 = new Role_Data();
            role_Data_1.key = RoleKey.FuHsi;

            Role_Data role_Data_2 = new Role_Data();
            role_Data_2.key = RoleKey.Nuwa;

            levelData.roles.Add(role_Data_1);
            levelData.roles.Add(role_Data_2);

            Event_Data event_Data1 = new Event_Data();
            event_Data1.key = EventKey.Love;
            event_Data1.RoleSum = 2;


            Event_Data event_Data2 = new Event_Data();

            levelData.events.Add(event_Data1);
            //levelData.events.Add(event_Data2);


            return levelData;
        }


        public void Dispatch_GameEvent(GameEvent gameEvent, params object[] pas)
        {
            gameDispatcher.Dispatch(gameEvent, pas);
        }


        

        





        #region PlayerInput

       
        /// 画面index 事件index 角色RoleKey 
        private void PlayerInput_SetRole(object[] param)
        {
            if (GameStructure == null)
            {
                return;
            }
            PictureEntity pictureEntity = param[0] as PictureEntity;
            Role_Data data = param[1] as Role_Data;
            int potIndex = (int)param[2];
            
            int pictureIndex = pictureEntity.Index;

            RoleKey roleKey = data.key;

            GameStructure.SetRole(pictureIndex, potIndex, roleKey);
        }
        /// 画面index 事件index  
        private void PlayerInput_RemoveRole(object[] param)
        {
            if (GameStructure == null)
            {
                return;
            }
            int pictureIndex = (int)param[0];
            int potIndex = (int)param[1];

            GameStructure.RemoveRole(pictureIndex,potIndex);
        }
        /// 画面index 事件EventKey
        private void PlayerInput_SetEvent(object[] par)
        {
            if (GameStructure == null)
            {
                return;
            }
            PictureEntity pictureEntity = par[0] as PictureEntity;
            Event_Data data = par[1] as Event_Data;
            int pictureIndex = pictureEntity.Index;

            EventKey eventKey = data.key;
            GameStructure.SetEvent(pictureIndex, eventKey);

        }
        /// 画面index
        private void PlayerInput_RemoveEvent(object[] param)
        {
            if (GameStructure == null)
            {
                return;
            }
            int pictureIndex = (int)param[0];


            GameStructure.RemoveEvent(pictureIndex);
        }

        #endregion


    }

   


    public class LevelData
    {
        ///画面数量
        public int allPictureSum = 3;
        

        public List<Event_Data> events = new List<Event_Data>();

        public List<Role_Data> roles = new List<Role_Data>();
    }

    public class GameStructure
    {
        /// <summary>
        /// 画面
        /// </summary>
        public List<BasePicture> allPictures = new List<BasePicture>();

        public Dictionary<RoleKey,Role_Data> allRole_Data = new Dictionary<RoleKey, Role_Data>();
        public Dictionary<EventKey,Event_Data> allEvent_Data = new Dictionary<EventKey, Event_Data>();
        public GameSys gameSys;

        public LevelData LevelData { get; private set; }

        private Dictionary<RoleKey, RoleState> allRoleState = new Dictionary<RoleKey, RoleState>();


        private List<RoleKey> tempList = new List<RoleKey>();

        

        public void FillData(LevelData levelData)
        {
            LevelData = levelData;

            allPictures.Clear();

            foreach (var item in levelData.roles)
            {
                allRole_Data.Add(item.key,item);
                allRoleState.Add(item.key, new RoleState());
            }

            foreach (var item in levelData.events)
            {
                allEvent_Data.Add(item.key, item);
            }

        }

        public void AddPicture(BasePicture _picture)
        {
            allPictures.Add(_picture);
        }

        public object GetRoleData(string roleKey)
        {

            return null;
        }

        public void Rest()
        {
            LevelData = null;

            foreach (var _Pictures in allPictures)
            {
                _Pictures.Rest();
            }
            allPictures.Clear();

        }
       
        public void SetRole(int pictureIndex, int potIndex, RoleKey roleKey)
        {
            if (pictureIndex >= allPictures.Count)
            {
                return;
            }

            BasePicture picture = allPictures[pictureIndex];

          
            if (!picture.CheckCanSetRole(roleKey, out int index_old, potIndex))
            {
                return;
            }

            if (index_old != -1)
            {
                //去除 该角色在本画面原先占住的位置 如将2位置换到1位置
                picture.RemoveRole(index_old);

            }

            Role_Data role_Data = allRole_Data[roleKey];

            RoleEntity role = gameSys.GetRoleEntity(role_Data);

            picture.SetRole(role, potIndex);

            RefactorPictures(pictureIndex);

        }

        public void SetEvent(int pictureIndex, EventKey eventKey)
        {
            if (pictureIndex>=allPictures.Count)
            {
                return;
            }

            BasePicture picture = allPictures[pictureIndex];


            SceneEventEntity eventEntity = gameSys.GetSceneEventEntity(allEvent_Data[eventKey]);

            picture.SetEvent(eventEntity);


            RefactorPictures(pictureIndex);
        }
        
        public void RemoveEvent(int pictureIndex)
        {

            if (pictureIndex >= allPictures.Count)
            {
                return;
            }

            BasePicture picture = allPictures[pictureIndex];

            picture.RemoveEvent();

            RefactorPictures(pictureIndex);

        }

        public void RemoveRole(int pictureIndex, int potIndex)
        {
            if (pictureIndex >= allPictures.Count)
            {
                return;
            }

            BasePicture picture = allPictures[pictureIndex];

            picture.RemoveRole(potIndex);

            RefactorPictures(pictureIndex);
        }


        /// <summary>
        /// 刷新 画面
        /// </summary>
        private void RefactorPictures(int pictureIndex)
        {
            foreach (var state in allRoleState)
            {
                state.Value.Rest();
            }

            for (int i = pictureIndex - 1; i >= 0; i--)
            {
                BasePicture picture = allPictures[i];

                foreach (var item in picture.Roles)
                {
                    if (!tempList.Contains(item.Key))
                    {
                        tempList.Add(item.Key);

                        item.Value.State.CopyTo(allRoleState[item.Key]);
                    }
                } 

            }
            tempList.Clear();

            for (int i = pictureIndex; i < allPictures.Count; i++)
            {
                BasePicture picture = allPictures[i];

                picture.Refactor(allRoleState);

            }




        }


    }


    
}