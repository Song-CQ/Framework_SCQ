using UnityEngine;

namespace FuturePlugin
{
    public class FutureFrame : MonoBehaviour
    {
        public static FutureFrame Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}

