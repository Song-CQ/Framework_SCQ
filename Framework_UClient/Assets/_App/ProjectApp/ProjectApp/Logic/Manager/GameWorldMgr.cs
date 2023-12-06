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

   
            gameWord.FillData(0);

        }
    }


    public class LevelData
    {
        public int allPictureSum = 6;
        

    }

    

    
}