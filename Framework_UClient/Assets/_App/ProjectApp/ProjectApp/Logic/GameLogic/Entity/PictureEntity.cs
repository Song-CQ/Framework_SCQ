/****************************************************
    文件: PictureEntity.cs
    作者: Clear
    日期: 2023/12/6 11:28:3
    类型: 逻辑脚本
    功能: 画面实体
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{

    public class PictureEntity: BasePicture
    {
        public Transform Transform {  get; private set; }
        public Transform RolesTrf {  get; private set; }
        public Transform EventTrf {  get; private set; }
        public UIEventListener EventListener { get; private set; }
        private SceneEventEntity sceneEventEntity => SceneEvent as SceneEventEntity;


        public override void Init()
        {
            base.Init();
            Transform = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.PictureEntityPath).transform;

            EventTrf = Transform.Find("Event").transform;
            RolesTrf = Transform.Find("AllRole").transform;

            EventListener = UIEventListener.GetEventListener(Transform);

        }


        public override bool SetRole(IRole role, int potIndex = -1)
        {
            bool isres =  base.SetRole(role, potIndex);

            if (isres) 
            {
                RoleEntity roleEntity = role as RoleEntity;

                roleEntity.Entity.transform.SetParent(RolesTrf);
                roleEntity.Entity.transform.localScale = Vector3.one;

                roleEntity.Entity.transform.localPosition = sceneEventEntity.GetRolePot(potIndex);
            }
            return isres;
        }

        public override void Show(int index)
        {
            base.Show(index);
            Transform.SetActive(true);
          
            Vector2 pot = new Vector2(GameWordEntity.p_w * (index % 3), - GameWordEntity.p_h * (index / 3));
            Transform.localPosition = GameWordEntity.PictureRoodPot + pot;
        }

        public override void Rest()
        {
            base.Rest();
            Transform.SetActive(false);



        }

       
    }
}