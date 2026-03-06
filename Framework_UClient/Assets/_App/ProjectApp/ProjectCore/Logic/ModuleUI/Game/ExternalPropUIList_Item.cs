/****************************************************
    文件: ExternalPropItem.cs
    作者: Clear
    日期: 2026/2/8 16:33:27
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using FutureCore;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class PropData : ItemData
    {
        public ExternalProp type;
        public int Sum;

    }
    public class ExternalPropUIList_Item : BaseUIList_Item
    {
        public Image icon;

        public TextMeshProUGUI sumText;

        public Transform sumTrf;
        public Transform btn_AddTrf;

        public void OnClickAddSum()
        {
            
        }

        public override void Initialize(int index, ItemData data)
        {
            base.Initialize(index, data);
            PropData propData = data as PropData;


            icon.sprite = GameTool.GetSprite(propData.type);
            int sum = propData.Sum;
            if (sum == 0)
            {
                btn_AddTrf.gameObject.SetActive(true);
                sumTrf.gameObject.SetActive(false);
            }
            else
            { 
                sumText.text = sum.ToString();
                btn_AddTrf.gameObject.SetActive(false);
                sumTrf.gameObject.SetActive(true);
            }


        }

    }
}