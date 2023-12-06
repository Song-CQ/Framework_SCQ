/****************************************************
    文件: PictureEntity.cs
    作者: Clear
    日期: 2023/12/6 11:28:3
    类型: 逻辑脚本
    功能: 画面实体
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class PictureEntity: IPicture
    {
        public Transform Transform {  get; private set; }
        public UIEventListener EventListener { get; private set; }

        public IEvent Scene { get; private set; }

        public void Init()
        {
            Transform = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>("Prefabs/GamePrefabs/PictureEntity")).transform;
            EventListener = UIEventListener.GetEventListener(Transform);

        }

        public void SetScene(IEvent newScene)
        {
            Scene?.Exit();

            Scene = newScene;
            Scene.Show();

        }

        
        public void Show(int index)
        {
            Transform.SetActive(true);
            Transform.SetParent(GameWordEntity.AllPictureEntity);
            Vector2 pot = new Vector2(TestMgr.Instance.w * (index % 3), -TestMgr.Instance.h * (index / 3));
            Transform.localPosition = GameWordEntity.PictureRoodPot + pot;
        }
        public void Rest()
        {
            Scene?.Exit();
            Scene = null;

            Transform.SetParent(GameWordEntity.EntityPool);
            Transform.localPosition = Vector3.zero;
            Transform.SetActive(false);
        }
    }
}