using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    /// <summary>
    /// 编辑器样式预览器
    /// </summary>
    public class EditorStyleViewer : EditorWindow
    {
        static EditorWindow window;

        [MenuItem("Assets/[FC Window]/编辑器内置样式预览器", false, -99)]

        static void OpenWindowAss()
        {
            OpenWindow();
        }

        [MenuItem("GameObject/[FC Window]/编辑器内置样式预览器", false, -99)]
        private static void GoOpenWindowAss()
        {
            OpenWindow();
        }


        [MenuItem("[FC Window]/编辑器内置样式预览器")]
        static void OpenWindow()
        {
            if (window == null)
            {
                window = CreateWindow<EditorStyleViewer>("编辑器内置样式预览器");
            }
           

            
            window.minSize = new Vector2(900, 300);
            window.Show();
            window.Focus();
        }

        private void OnEnable()
        {
            GUIStyleWindow.Instance.InitEvent(this);
        }


        private void OnGUI()
        {

            GUIStyleWindow.Instance.InitEditorDemo();

        }

       
    }
}