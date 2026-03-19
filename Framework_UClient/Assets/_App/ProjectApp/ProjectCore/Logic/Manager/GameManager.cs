/****************************************************
    文件: GameManager.cs
    作者: Clear
    日期: 2023/12/4 14:59:5
    类型: 逻辑脚本
    功能: 游戏核心管理器
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameManager : BaseMgr<GameManager>
    {
        public EliminateGameCore gameCore;

        protected override void New()
        {
            base.New();
        }
        public override void Init()
        {
            base.Init();

        }

        public override void StartUp()
        {
            base.StartUp();


        }

        public void EnterGame()
        {
            SceneMgr.Instance.AdditiveScene(2, LoadComplete, null);


        }

        public void ExitGame()
        {
            SceneMgr.Instance.UnAdditiveScene(2, null, null);
        }

        private void LoadComplete(object obj)
        {
            //UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.GameUI_Open);

            gameCore = GameObject.FindObjectOfType<EliminateGameCore>(true);
           

            gameCore.Init(MainUI.GameLV);

        }
    }


    
}