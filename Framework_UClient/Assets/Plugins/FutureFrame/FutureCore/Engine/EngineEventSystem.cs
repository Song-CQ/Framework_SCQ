using UnityEngine;
using UnityEngine.EventSystems;

namespace FuturePlugin
{
    public class EngineEventSystem : MonoBehaviour
    {
        public static EngineEventSystem Instance { get; private set; }

        [HideInInspector]
        public GameObject eventObj;
        [HideInInspector]
        public EventSystem eventSystem;
        [HideInInspector]
        public StandaloneInputModule inputModule;

        private void Awake()
        {
            Instance = this;
            Unity3dUtil.SetDontDestroyOnLoad(gameObject);

            Init();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Init()
        {
            eventObj = Instantiate(Resources.Load<GameObject>("Preset/EngineEventSystem/EngineEventSystem"));
            eventObj.transform.SetParent(transform, false);
            eventSystem = eventObj.GetComponent<EventSystem>();
            inputModule = eventObj.GetComponent<StandaloneInputModule>();

            LogUtil.Log("[EngineEventSystem]Init");
        }
    }
}