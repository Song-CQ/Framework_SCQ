/****************************************************
    鏂囦欢: BetterBeeFlight.cs
    浣滆? Clear
    鏃ユ湡: 2026/2/11 2:10:17
    绫诲瀷: 閫昏緫鑴氭湰
    鍔熻兘: Nothing
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    using UnityEngine;
    using DG.Tweening;
    using System;

    public class BetterBeeFlight : MonoBehaviour
    {

        [Header("鐩?爣璁剧疆")]
        public Transform hiveTarget;
        public float flightTime = 3f;

        [Header("椋炶?鏁堟灉")]
        public bool useCurvePath = true;
        public float curveHeight = 2f;
        public bool rotateToTarget = true;

        private Action _cb;

        private Vector3 o_Pot;
        private bool isInit = false;

        void Awake()
        {
            Init();
        }
        public void Init()
        {
            if (isInit) return;
            o_Pot = transform.localPosition;
            LogUtil.LogFormat(transform.localPosition+"璁剧疆 "+o_Pot.ToString());
            isInit = true;
        }


        public void FlyToHive(Transform trf)
        {
            Init();
            LogUtil.LogFormat(transform.localPosition+" 褰撳墠"+o_Pot.ToString());

            hiveTarget = trf;

            if (hiveTarget == null)
            {
                Debug.LogError("璇疯?缃?渹宸㈢洰鏍囷紒");
                return;
            }

            if (useCurvePath)
            {
                // 鏇茬嚎璺?緞椋炶?
                Vector3[] path = new Vector3[3];
                path[0] = transform.position;
                path[1] = (transform.position + hiveTarget.position) / 2 + Vector3.up * curveHeight;
                path[2] = hiveTarget.position;

                transform.DOPath(path, flightTime, PathType.CatmullRom)
                    .SetEase(Ease.InOutSine).onComplete = Complete;
            }
            else
            {
                // 鐩寸嚎椋炶?
                transform.DOMove(hiveTarget.position, flightTime)
                    .SetEase(Ease.InOutSine);
            }

            // 鏃嬭浆鐪嬪悜鐩?爣
            if (rotateToTarget)
            {
                // transform.DOLookAt(hiveTarget.position, 0.5f);
                // 鑾峰彇2D鏂瑰悜
                Vector2 direction = hiveTarget.position - transform.position;

                // 鍒涘缓浠庡彸鏂瑰悜鍒扮洰鏍囨柟鍚戠殑鏃嬭浆锛堝彧缁昛杞达級
                Quaternion targetRotation = Quaternion.FromToRotation(Vector2.up, direction);

                // 搴旂敤鏃嬭浆
                transform.DORotateQuaternion(targetRotation, 0.5f);
            }
        }

        private void Complete()
        {
            _cb?.Invoke();

            Reset();




        }

        private void Reset()
        {
            _cb = null;



            transform.localPosition = o_Pot;


        }

        // 鐐瑰嚮閲嶆柊椋炶?锛堢敤浜庢祴璇曪級
        void OnMouseDown()
        {
            // // 閲嶇疆浣嶇疆
            // transform.position = new Vector3(Random.Range(-3f, 3f), 1, Random.Range(-3f, 3f));

            // // 閲嶆柊椋炶?
            // FlyToHive();
        }

        public void AddCB(Action value)
        {
            if (_cb != null)
            {
                _cb += value;
            }
            else
            {
                _cb = value;
            }
        }
    }
}