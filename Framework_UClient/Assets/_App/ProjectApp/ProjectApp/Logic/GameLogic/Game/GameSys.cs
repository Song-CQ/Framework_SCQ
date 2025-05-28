/****************************************************
    文件: GameSys.cs
    作者: Clear
    日期: 2023/12/4 11:6:21
    类型: 逻辑脚本
    功能: 游戏系统
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameSys : BaseSystem
    {

        public class GameDispatcher : BaseDispatcher<GameDispatcher, GameEvent, object[]> { }

        public GameDispatcher gameDispatcher { private set; get; }

        private Dictionary<EventKey, EventCode> allEventCodeDic;


        public override void Init()
        {
            base.Init();

            InitEventCode();

            gameDispatcher = new GameDispatcher();


        }

        private void InitEventCode()
        {
            allEventCodeDic = new Dictionary<EventKey, EventCode>();

            allEventCodeDic.Add(EventKey.Empty, new Empty_EventCode());
            allEventCodeDic.Add(EventKey.Love, new Love_EventCode());


        }

        public override void Start()
        {
            base.Start();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Dispose()
        {
            base.Dispose();
        }






        public SceneEventEntity GetSceneEventEntity(Event_Data _data)
        {
            SceneEventEntity entity = ObjectPoolStatic<SceneEventEntity>.Get();

            EventCode eventCode = GetEventCode(_data.Key);

            entity.Init(_data, eventCode);

            return entity;
        }



        private EventCode GetEventCode(EventKey key)
        {
            if (allEventCodeDic.ContainsKey(key))
            {
                return allEventCodeDic[key];
            }
            return new Empty_EventCode();
        }

        public RoleEntity GetRoleEntity(Role_Data _data)
        {
            RoleEntity roleEntity = new RoleEntity();

            roleEntity.Init(_data);

            return roleEntity;
        }

        public void AddListener(GameEvent setEvent, Action<object[]> paramCB)
        {
            gameDispatcher.AddListener(GameEvent.SetEvent, paramCB);
        }
        public void RemoveListener(GameEvent setEvent, Action<object[]> paramCB)
        {
            gameDispatcher.RemoveListener(GameEvent.SetEvent, paramCB);
        }

        public void Dispatch(GameEvent gameEvent, params object[] pas)
        {
            gameDispatcher.Dispatch(gameEvent, pas);
        }


        public bool CheckFinish(Dictionary<RoleKey, RoleState> allRoleState)
        {

            



            return false;
        }
        


    }


    

}