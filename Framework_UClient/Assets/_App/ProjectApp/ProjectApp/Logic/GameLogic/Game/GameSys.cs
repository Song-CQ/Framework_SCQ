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
using ProjectApp.Data;

namespace ProjectApp
{
    public class GameSys : BaseSystem
    {

        public class GameDispatcher : BaseDispatcher<GameDispatcher, GameEvent, object[]> { }

        public GameDispatcher gameDispatcher { private set; get; }

        private Dictionary<EventKey, EventCode> allEventCodeDic;

        private static Dictionary<string, EventKey> eventKeyDic = new Dictionary<string, EventKey>();
        private static Dictionary<string, RoleKey> roleKeyDic = new Dictionary<string, RoleKey>();


        public override void Init()
        {
            base.Init();

            InitEnum();

            InitEventCode();

            gameDispatcher = new GameDispatcher();




        }

        private void InitEnum()
        {
            foreach (var data in Enum.GetValues(typeof(EventKey)))
            {
                EventKey key = (EventKey)data;
                eventKeyDic.Add(key.ToString(), key);
            }

            foreach (var data in Enum.GetValues(typeof(RoleKey)))
            {
                RoleKey key = (RoleKey)data;
                roleKeyDic.Add(key.ToString(), key);
            }

        }

        private void InitEventCode()
        {
            allEventCodeDic = new Dictionary<EventKey, EventCode>();

            allEventCodeDic.Add(EventKey.Empty, new Meet_EventCode());
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
            return null;
        }

        public RoleEntity GetRoleEntity(Role_Data _data,RoleState state)
        {
            RoleEntity roleEntity = new RoleEntity();

            roleEntity.Init(_data,state);

            return roleEntity;
        }

        public void AddListener(GameEvent gameEvent, Action<object[]> paramCB)
        {
            gameDispatcher.AddListener(gameEvent, paramCB);
        }
        public void RemoveListener(GameEvent gameEvent, Action<object[]> paramCB)
        {
            gameDispatcher.RemoveListener(gameEvent, paramCB);
        }

        public void Dispatch(GameEvent gameEvent, params object[] pas)
        {
            gameDispatcher.Dispatch(gameEvent, pas);
        }


        public bool CheckFinish(Dictionary<RoleKey, RoleState> allRoleState, LevelData levelData)
        {


            foreach (var id in levelData.data.CheckConditionList)//角色
            {
                CheckConditionVO condition = ConfigDataMgr.Instance.GetConfigVO<CheckConditionVO>(ConfigVO.CheckCondition, id);

                if (condition.type == 0) // 以角色状态为检测主体 检测角色状态
                { 
                     if (!condition.rolekey.IsNullOrEmpty()) 
                    {
                        RoleKey rolekey = GameSys.GetRoleKey(condition.rolekey);

                        if (!CheckConditionFinish(condition, allRoleState[rolekey]))
                        {
                            return false;
                        }


                    }

                }

               




            }

            return true;
        }



        private bool CheckConditionFinish(CheckConditionVO condition, RoleState roleState)
        {
            if (allEventCodeDic.TryGetValue(GetEventKey(condition.eventKey), out EventCode code))
            {
                return code.CheckConditionFinish(condition, roleState);
            }

            LogUtil.LogError("未有该类型检测器，Type：" + condition.eventKey);

            return false;
        }

        public static EventKey GetEventKey(string KeyStr)
        {
            if (eventKeyDic.TryGetValue(KeyStr, out EventKey key))
            {
                return key;
            }

            LogUtil.LogError("转换成EventKey失败，请检查： " + KeyStr);

            return EventKey.Empty;
        }

        public static RoleKey GetRoleKey(string KeyStr)
        {
            if (roleKeyDic.TryGetValue(KeyStr, out RoleKey key))
            {
                return key;
            }

       

            LogUtil.LogError("转换成RoleKey失败，请检查： " + KeyStr);

            return RoleKey.Node;
        }






    }




}