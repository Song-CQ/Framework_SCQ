using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    public static class AssetBundleToolView
    {
        public static void InitData()
        {
            // 如果有需要初始化的数据，在这里添加
        }

        public static void OnGUI(System.Action closeAction)
        {
            if (GUILayout.Button("Open AssetBundles Window", GUILayout.Height(40), GUILayout.Width(180)))
            {
                BuildAssetBundleWnd.OpenWnd();
                closeAction?.Invoke();
            }
        }
    }
}