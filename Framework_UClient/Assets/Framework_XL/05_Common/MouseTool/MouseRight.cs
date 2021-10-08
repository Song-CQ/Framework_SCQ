using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XL.WinterProject.MainModule
{
    /// <summary>
    /// 鼠标右键
    /// </summary>
    public class MouseRight : MonoBehaviour 
   {

        private Transform parent;
        Vector3 vector;
        private void Awake()
        {
            parent = transform.parent;
        }
        private void Update()
        {
            
            if (Input.GetMouseButtonDown(1))
            {
                //记录初始鼠标位置
                vector = Input.mousePosition;
            }
            if (Input.GetMouseButton(1))
            {
                //鼠标当前位置-记录位置得到偏移量
                Vector2 offer = Input.mousePosition - vector;
                //用偏移量旋转
                parent.transform.eulerAngles += new Vector3(0,-offer.x, offer.y)/2;
                vector = Input.mousePosition;
                
            }
           
        }
    }
}
