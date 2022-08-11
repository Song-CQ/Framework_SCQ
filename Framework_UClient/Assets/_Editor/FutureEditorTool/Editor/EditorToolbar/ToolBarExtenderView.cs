using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FutureEditor
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle ToolBarExtenderBtnStyle;

		static ToolbarStyles()
		{
			ToolBarExtenderBtnStyle = new GUIStyle("Command")
			{
				fontSize = 12,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Normal,
				fixedWidth = 60
			};
		}
	}

	[InitializeOnLoad]
	public class ToolbarExtenderView
	{
		static ToolbarExtenderView()
		{
			
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUILeft);
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUIRight);
		}

		static void OnToolbarGUILeft()
		{
			//下面只是Demo，可随便更改
			GUILayout.Space(100);
			string finalStr = $"AppName : {ProjectApp.AppFacade.AppDesc}";

			GUILayout.Label(finalStr, new GUIStyle("SelectionRect"));

			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Play MainScene", EditorGUIUtility.FindTexture("PlayButton"), "跳转主场景并播放")))
			{
				EditorSceneTool.PlayMainScene();
            }
			
			GUILayout.Space(30);
		}

		static void OnToolbarGUIRight()
		{
			//下面只是Demo，可随便更改
			GUILayout.Space(1);
			if (GUILayout.Button(new GUIContent("Test Scene", EditorGUIUtility.FindTexture("Button"), "跳转测试场景")))
			{
				EditorSceneTool.LoadTestScene();
			}

			if (GUILayout.Button(new GUIContent("Open Framework View", EditorGUIUtility.FindTexture("Button"), "打开工具界面")))
			{
				FrameworkToolView.OpenFrameworkWin();
			}

			GUILayout.FlexibleSpace();
			
			//if (GUILayout.Button(new GUIContent("SVN更新", "更新当前的客户端"), ToolbarStyles.ToolBarExtenderBtnStyle))
			//{
			//	SVNUtils.UpdateSVNProject();
			
			//	Debug.Log("TODO : SVN更新");
			//}
			//if (GUILayout.Button(new GUIContent("SVN提交", "提交客户端"), ToolbarStyles.ToolBarExtenderBtnStyle))
			//{
			//	SVNUtils.UpdateCommitProject();
			//	Debug.Log("TODO : SVN提交");
			//}
			//Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 10, new GUIStyle("MiniSliderHorizontal"), new GUIStyle("MinMaxHorizontalSliderThumb"), GUILayout.MinWidth(200), GUILayout.MinHeight(20));
		}
	}

	

}