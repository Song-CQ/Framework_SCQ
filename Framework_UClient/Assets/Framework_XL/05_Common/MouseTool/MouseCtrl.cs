using UnityEngine;

namespace XL.WinterProject.MainModule
{
    public class MouseCtrl : MonoBehaviour
    {
        private Vector3 TargetScreenSpace;// 目标物体的屏幕空间坐标
        private Vector3 TargetWorldSpace;// 目标物体的世界空间坐标
        private Transform trans;// 目标物体的空间变换组件
        private Vector3 MouseScreenSpace;// 鼠标的屏幕空间坐标
        private Vector3 Offset;// 偏移


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if(Input.GetMouseButtonUp(0)){
            //	GameObject.Find("Main Camera").GetComponent<UIset>().enabled = true;
            //}

            if (Input.GetMouseButton(0))
            {

                // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）

                MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, TargetScreenSpace.z);

                // 把鼠标的屏幕空间坐标转换到世界空间坐标（Z值使用目标物体的屏幕空间坐标），加上偏移量，以此作为目标物体的世界空间坐标

                TargetWorldSpace = Camera.main.ScreenToWorldPoint(MouseScreenSpace) + Offset;

                // 更新目标物体的世界空间坐标 

                trans.position = TargetWorldSpace;

                // 等待固定更新 


            }
        }
        void Awake()
        {
            trans = transform.parent;



        }

        void OnMouseDown()

        {


            // 把目标物体的世界空间坐标转换到它自身的屏幕空间坐标 

            TargetScreenSpace = Camera.main.WorldToScreenPoint(trans.position);

            // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标） 

            MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, TargetScreenSpace.z);

            // 计算目标物体与鼠标物体在世界空间中的偏移量 

            Offset = trans.position - Camera.main.ScreenToWorldPoint(MouseScreenSpace);

            // 鼠标左键按下 


        }
    }
}