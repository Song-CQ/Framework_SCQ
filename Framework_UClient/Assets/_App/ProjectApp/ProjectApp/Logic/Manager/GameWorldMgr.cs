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
    public class GameMgr : BaseMgr<GameMgr>
    {
        private GameSys _gameSys;

        private GameWord gameWord;

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


        }
    }


    public class GameWord
    {
        public static Transform WordRood;

        public static Vector2 PictureRoodPot = Vector2.zero;

        public void Init()
        {
            WordRood = new GameObject("WordRood").transform;
            WordRood.transform.position = new Vector3(0, 0, 0);
            

        }


       

        public void Rest()
        {
            


        }

    }
}