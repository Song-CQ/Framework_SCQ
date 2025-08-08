/****************************************************
    文件: GameStructure.cs
    作者: Clear
    日期: 2025/8/8 17:23:4
    类型: 游戏结构
    功能: 一句游戏的结构载体
*****************************************************/
using System;
using UnityEngine;

namespace ProjectApp
{
    public class GameStructure
    {
        private GameWord gameWord;


        public void Init(GameWord _gameWord)
        {
            gameWord = _gameWord;

        }

        public void Fill()
        {

        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}