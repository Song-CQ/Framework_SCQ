/****************************************************
    文件: RoleAvatar.cs
    作者: Clear
    日期: 2025/5/26 16:57:2
    类型: 逻辑脚本
    功能: 角色外形脚本
*****************************************************/
using System;
using TMPro;
using UnityEngine;

namespace ProjectApp
{
    public class RoleAvatar : MonoBehaviour
    {

        private TextMeshPro stateTextMeshPro;
        private TextMeshPro descTextMeshPro;

        private Animation animation;

        [SerializeField]
        private Role_Data role_Data;

        [SerializeField]
        private RoleState role_State;




        private void Awake()
        {
            stateTextMeshPro = transform.Find("state").GetComponent<TextMeshPro>();

            descTextMeshPro = transform.Find("desc").GetComponent<TextMeshPro>();

            stateTextMeshPro.text = string.Empty;
            descTextMeshPro.text = string.Empty;
            

        }



        public void Init(Role_Data data)
        {
            Transform role = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.RoleAniPath + data.Key.ToString()).transform;
            role.SetParent(transform);
            role.localScale = Vector3.one * 0.15f;
            role.localPosition = Vector3.zero;

            animation = role.GetComponent<Animation>();
            animation.Play("ideo_01");

            this.role_Data = data;



        }

        public void Clear()
        {
            stateTextMeshPro.text = string.Empty;
            descTextMeshPro.text = string.Empty;

            animation.gameObject.SetActive(false);
            GameWorldMgr.Instance.GameEntity.ReleasePrefabGo(GameWordEntity.RoleAniPath + role_Data.Key.ToString(), animation.gameObject);
            animation = null;
            role_Data = null;
            role_State = null;



        }

        public void SetState(RoleState state)
        {
            role_State = state;
            string val = state.GetString();
            stateTextMeshPro.text = val;

            if (state.GetState(StateKey.Hand))
            {
                animation.Play("hand_02");

            }
            else
            {
                animation.Play("ideo_01");
                

            }

        }
    }
}