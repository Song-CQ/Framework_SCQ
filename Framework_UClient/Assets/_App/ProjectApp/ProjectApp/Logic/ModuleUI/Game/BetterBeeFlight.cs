/****************************************************
    文件: BetterBeeFlight.cs
    作者: Clear
    日期: 2026/2/11 2:10:17
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    using UnityEngine;
    using DG.Tweening;

    public class BetterBeeFlight : MonoBehaviour
    {
        [Header("目标设置")]
        public Transform hiveTarget;
        public float flightTime = 3f;

        [Header("飞行效果")]
        public bool useCurvePath = true;
        public float curveHeight = 2f;
        public bool rotateToTarget = true;

        void Start()
        {
            FlyToHive();
        }

        public void FlyToHive()
        {
            if (hiveTarget == null)
            {
                Debug.LogError("请设置蜂巢目标！");
                return;
            }

            if (useCurvePath)
            {
                // 曲线路径飞行
                Vector3[] path = new Vector3[3];
                path[0] = transform.position;
                path[1] = (transform.position + hiveTarget.position) / 2 + Vector3.up * curveHeight;
                path[2] = hiveTarget.position;

                transform.DOPath(path, flightTime, PathType.CatmullRom)
                    .SetEase(Ease.InOutSine);
            }
            else
            {
                // 直线飞行
                transform.DOMove(hiveTarget.position, flightTime)
                    .SetEase(Ease.InOutSine);
            }

            // 旋转看向目标
            if (rotateToTarget)
            {
                transform.DOLookAt(hiveTarget.position, 0.5f);
            }
        }

        // 点击重新飞行（用于测试）
        void OnMouseDown()
        {
            // 重置位置
            transform.position = new Vector3(Random.Range(-3f, 3f), 1, Random.Range(-3f, 3f));

            // 重新飞行
            FlyToHive();
        }
    }
}