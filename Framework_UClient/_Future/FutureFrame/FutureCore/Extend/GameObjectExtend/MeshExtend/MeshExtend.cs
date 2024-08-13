/****************************************************
    文件: MeshExtend.cs
    作者: Clear
    日期: 2022/8/12 21:10:49
    类型: 框架核心脚本(请勿修改)
    功能: 网格工具扩展
*****************************************************/
using UnityEngine;

namespace FutureCore
{
    public static class MeshExtend
    {

        public static Mesh CreateCylinderMesh(float radius, float Height, int Segments)
        {
            //vertices:
            int vertices_count = Segments * 2;
            Vector3[] vertices = new Vector3[vertices_count];
            //vertices[0] = Vector3.zero;
            float angledegree = 360.0f;
            float angleRad = Mathf.Deg2Rad * angledegree;
            float angleCur = angleRad;
            float angledelta = angleRad / Segments;
            for (int i = 0; i < vertices_count; i++)
            {
                float cosA = Mathf.Cos(angleCur);
                float sinA = Mathf.Sin(angleCur);

                vertices[i] = new Vector3(radius * cosA, Height / 2, radius * sinA);
                i++;
                vertices[i] = new Vector3(radius * cosA, -Height / 2, radius * sinA);
                angleCur -= angledelta;
            }

            //triangles
            int triangle_count = Segments * 3 * 2;
            int[] triangles = new int[triangle_count + 12];


            int vert = 0;
            int idx = 0;
            for (int i = 0; i < Segments; i++)     //因为该案例分割了60个三角形，故最后一个索引顺序应该是：0 60 1；所以需要单独处理
            {
                //只有顺时针旋转排序的三角形才会被绚烂，法线 单面


                triangles[idx++] = CheckMaxVert(vert, vertices.Length);
                triangles[idx++] = CheckMaxVert(vert + 1, vertices.Length);
                triangles[idx++] = CheckMaxVert(vert + 2, vertices.Length);

                triangles[idx++] = CheckMaxVert(vert + 1, vertices.Length);
                triangles[idx++] = CheckMaxVert(vert + 3, vertices.Length);
                triangles[idx++] = CheckMaxVert(vert + 2, vertices.Length);

                //为了完成闭环，将最后一个三角形单独拎出来

                //所以上面是是单面 双面是双倍的三角形数量

                //triangles[idx++] = vert;
                //triangles[idx++] = vert + 2;
                //triangles[idx++] = vert + 1;

                //triangles[idx++] = vert + 1;
                //triangles[idx++] = vert + 2;
                //triangles[idx++] = vert + 3;

                vert += 2;
            }
            ////圆线
            //int addOf = (Segments / 6);
            //if (addOf % 2 != 0)
            //{
            //    addOf ++;
            //}
            //for (int i = 0; i < 2; i ++)
            //{
            //    triangles[triangle_count +i * 6] = 0 / 3 * Segments * 2 + i * addOf;
            //    triangles[triangle_count + i * 6 + 1] = 1 * Segments / 3 * 2 + i * addOf;
            //    triangles[triangle_count + i * 6 + 2] = 2  * Segments / 3 * 2 + i * addOf;

            //    triangles[triangle_count + i * 6 + 3] = 0  * Segments / 3 * 2 +1 + i * addOf;
            //    triangles[triangle_count + i * 6 + 4] = 1  * Segments / 3 * 2 +1 + i * addOf;
            //    triangles[triangle_count + i * 6 + 5] = 2 * Segments / 3 * 2 +1 + i * addOf;
            //}

            //uv: 
            Vector2[] uvs = new Vector2[vertices_count];
            float uvSetup = 1.0f / Segments;
            int iduv = 0;
            for (int i = 0; i < vertices_count; i = i + 2)
            {
                uvs[i] = new Vector2(uvSetup * iduv, 1);
                uvs[i + 1] = new Vector2(uvSetup * iduv, 0);
                iduv++;
            }



            //负载属性与mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = vertices;
            mesh.uv = uvs;
            return mesh;
        }

        private static int CheckMaxVert(int vertIndex, int MaxVert)
        {
            if (vertIndex >= MaxVert)
            {
                vertIndex -= MaxVert;
            }
            return vertIndex;
        }



    }
}