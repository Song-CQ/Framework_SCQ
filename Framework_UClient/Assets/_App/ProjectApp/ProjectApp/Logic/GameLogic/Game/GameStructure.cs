/****************************************************
    文件: GameStructure.cs
    作者: Clear
    日期: 2025/5/27 17:12:4
    类型: 逻辑脚本
    功能: 游戏结构
*****************************************************/
using System;
using System.Collections.Generic;
using ILRuntime.Runtime;
using ProjectApp.Data;
using UnityEngine;

namespace ProjectApp
{
    public class LevelData
    {

        public LevelDataVO data { private set; get;}
        //标题
        public string title;
        ///画面数量
        public int allPictureSum = 3;


        public List<Event_Data> events = new List<Event_Data>();

        public List<Role_Data> roles = new List<Role_Data>();


        public LevelData(LevelDataVO _data)
        {
            data = _data;

            title = data.title;
            allPictureSum = data.PictureSum;      

            
             

        }
    }

    public class GameStructure
    {
        /// <summary>
        /// 画面
        /// </summary>
        public List<BasePicture> allPictures = new List<BasePicture>();

        public Dictionary<RoleKey, Role_Data> allRole_Data = new Dictionary<RoleKey, Role_Data>();
        public Dictionary<EventKey, Event_Data> allEvent_Data = new Dictionary<EventKey, Event_Data>();
        

        public LevelData LevelData { get; private set; }

        private Dictionary<RoleKey, RoleState> allRoleState = new Dictionary<RoleKey, RoleState>();


        private List<RoleKey> tempList = new List<RoleKey>();

        private GameSys gameSys;


        public GameStructure(GameSys sys)
        {
            gameSys = sys;
        }

        private void AddListener()
        {


        }

        

        private void RemoveListener()
        {
           

        }

      
        


       
        public void FillData(LevelData levelData)
        {
            LevelData = levelData;

            allPictures.Clear();

            foreach (var item in levelData.roles)
            {
                allRole_Data.Add(item.Key, item);
                allRoleState.Add(item.Key, new RoleState());
            }

            foreach (var item in levelData.events)
            {
                allEvent_Data.Add(item.Key, item);
            }

            AddListener();

        }

        public void Rest()
        {
            LevelData = null;

            foreach (var _Pictures in allPictures)
            {
                _Pictures.Rest();
            }
            allPictures.Clear();


            RemoveListener();

        }




        public void AddPicture(BasePicture _picture)
        {
            allPictures.Add(_picture);
        }

        public object GetRoleData(string roleKey)
        {

            return null;
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

            RoleEntity role = gameSys.GetRoleEntity(role_Data,allRoleState[roleKey]);

            picture.SetRole(role, potIndex);

            RefactorPictures(pictureIndex);

        }

        public void SetEvent(int pictureIndex, EventKey eventKey)
        {
            if (pictureIndex >= allPictures.Count)
            {
                return;
            }

            BasePicture picture = allPictures[pictureIndex];

            if (picture.SceneEvent.Key == eventKey)
            {
                return;
            }


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

        public void RemoveRole(int pictureIndex, RoleKey key)
        {
            if (pictureIndex >= allPictures.Count)
            {
                return;
            }

            BasePicture picture = allPictures[pictureIndex];

            picture.RemoveRole(key);

            RefactorPictures(pictureIndex);
        }

        /// <summary>
        /// 刷新 画面
        /// </summary>
        private void RefactorPictures(int pictureIndex)
        {
            // foreach (var state in allRoleState)
            // {
            //     state.Value.Rest();
            // }


            // //从当前节点往上寻找角色状态
            // for (int i = pictureIndex - 1; i >= 0; i--)
            // {
            //     BasePicture picture = allPictures[i];

            //     foreach (var item in picture.Roles)
            //     {
            //         if (!tempList.Contains(item.Key))
            //         {
            //             tempList.Add(item.Key);

            //             item.Value.State.CopyTo(allRoleState[item.Key]);
            //         }
            //     }

            // }
            // tempList.Clear();

            
            for (int i = pictureIndex; i < allPictures.Count; i++)
            {
                BasePicture picture = allPictures[i];

                picture.Refactor(allRoleState);

            }

            bool isFinish = gameSys.CheckFinish(allRoleState,LevelData);

            if (isFinish)
            {
                gameSys.Dispatch(GameEvent.GameFinish);
            }




        }

        
    }


}