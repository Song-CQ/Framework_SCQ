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
        private GameWordEntity gameWord;

        public GameStructure GameStructure { private set; get; }

        private class PlayerInputDispatcher : BaseDispatcher<PlayerInputDispatcher, PlayerInput, int[]> { }
        private PlayerInputDispatcher playerInputDispatcher;


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

            playerInputDispatcher = new PlayerInputDispatcher();

            playerInputDispatcher.AddListener(PlayerInput.SetEvent, PlayerInput_SetEvent);         //param = 画面index 事件EventKey
            playerInputDispatcher.AddListener(PlayerInput.SetRole, PlayerInput_SetRole);           //param = 画面index 事件index 角色RoleKey 
            playerInputDispatcher.AddListener(PlayerInput.RemoveEvent, PlayerInput_RemoveEvent);   //param = 画面index
            playerInputDispatcher.AddListener(PlayerInput.RemoveRole, PlayerInput_RemoveRole);     //param = 画面index 事件index 

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

            gameStructure.gameSys = _gameSys;
            return gameStructure;
        }


        public void Dispatch(PlayerInput playerInput, params int[] pas)
        {
            playerInputDispatcher.Dispatch(playerInput, pas);
        }







        #region PlayerInput

       
        /// 画面index 事件index 角色RoleKey 
        private void PlayerInput_SetRole(int[] param)
        {
            if (gameStructure == null)
            {
                return;
            }
            int pictureIndex = param[0];
            int potIndex = param[1];
            RoleKey roleKey = (RoleKey)param[2];

            gameStructure.SetRole(pictureIndex, potIndex, roleKey);
        }
        /// 画面index 事件index  
        private void PlayerInput_RemoveRole(int[] param)
        {
            if (gameStructure == null)
            {
                return;
            }
            int pictureIndex = param[0];
            int potIndex = param[1];

            gameStructure.RemoveRole(pictureIndex,potIndex);
        }
        /// 画面index 事件EventKey
        private void PlayerInput_SetEvent(int[] param)
        {
            if (gameStructure == null)
            {
                return;
            }
            int pictureIndex = param[0];

            EventKey eventKey = (EventKey)param[1];
            gameStructure.SetEvent(pictureIndex, eventKey);

        }
        /// 画面index
        private void PlayerInput_RemoveEvent(int[] param)
        {
            if (gameStructure == null)
            {
                return;
            }
            int pictureIndex = param[0];

  
            gameStructure.RemoveEvent(pictureIndex);
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