/****************************************************
    文件: Entity.cs
    作者: Clear
    日期: 2025/8/8 17:33:24
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{

    public interface IGameComponent
    {
        Entity Entity { set; get; }

        bool Enabled { get; set; }

        void Dispose();
    }



    public class Entity
    {

        public List<IGameComponent> components = new List<IGameComponent>();


        public virtual void Init()
        {

        }


        public virtual void Reset()
        {

            foreach (var item in components)
            {
                item.Dispose();
            }
            components.Clear();
                     
        }





    }
}