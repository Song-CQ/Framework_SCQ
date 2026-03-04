using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    public static class SVNToolView
    {
        public static void InitData()
        {
            // 如果有需要初始化的数据，在这里添加
        }

        public static void OnGUI(System.Action closeAction)
        {
            if (GUILayout.Button("更新Assets目录", GUILayout.Height(40), GUILayout.Width(150)))
            {
                closeAction?.Invoke();
                SVNUtils.UpdateSVNProject_Assets();
            }
            
            if (GUILayout.Button("提交Assets目录", GUILayout.Height(40), GUILayout.Width(150)))
            {
                closeAction?.Invoke();
                SVNUtils.CommitProject_Assets();
            }
        }
    }
}