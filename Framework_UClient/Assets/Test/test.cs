/****************************************************
    文件: test.cs
    作者: Clear
    日期: 2022/8/12 16:44:29
    类型: 逻辑脚本
    功能: 测试
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class test : MonoBehaviour
    {

        public float ThisRadius = 10f;    //半径  
        public int Segments = 600;   //分割数  
        public float Height = 2;           ///3.1415926f;
        private MeshFilter meshFilter;


        void Start()
        {
            //Debug.Log("hello unity: ");
            meshFilter = GetComponent<MeshFilter>();
            //圆柱，长方体，平面
           
            //meshFilter.mesh = Createmesh(Radius,  Height);
            //meshFilter.mesh = Createmeshp(Radius, Height);

        }

        

        
        // Update is called once per frame
        void Update()
        {
            meshFilter.mesh = MeshExtend.CreateCylinderMesh(ThisRadius ,Height,Segments);
        }


        private void OnDrawGizmos()
        {
            Mesh mesh = MeshExtend.CreateCylinderMesh(ThisRadius, Height,Segments);




            Gizmos.DrawWireMesh(mesh,Vector3.zero);

        }


    }
 
}