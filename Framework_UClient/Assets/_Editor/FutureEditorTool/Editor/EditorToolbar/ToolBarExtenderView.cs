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

		static bool m_enabled;

		static bool Enabled
		{
			get { return m_enabled; }
			set
			{
				m_enabled = value;
				EditorPrefs.SetBool("SceneViewFocuser", value);
			}
		}

	

		static void OnPauseChanged(PauseState obj)
		{
			if (Enabled && obj == PauseState.Unpaused)
			{
				// Not sure why, but this must be delayed
				EditorApplication.delayCall += EditorWindow.FocusWindowIfItsOpen<SceneView>;
			}
		}

		static void OnPlayModeChanged(PlayModeStateChange obj)
		{
			if (Enabled && obj == PlayModeStateChange.EnteredPlayMode)
			{
				EditorWindow.FocusWindowIfItsOpen<SceneView>();
			}
		}

		static void OnToolbarGUI()
		{
			var tex = EditorGUIUtility.IconContent(@"UnityEditor.SceneView").image;

			GUI.changed = false;

			GUILayout.Toggle(m_enabled, new GUIContent(null, tex, "Focus SceneView when entering play mode"), "Command");
			if (GUI.changed)
			{
				Enabled = !Enabled;
			}
		}


		static ToolbarExtenderView()
		{

			m_enabled = EditorPrefs.GetBool("SceneViewFocuser", false);
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
			EditorApplication.pauseStateChanged += OnPauseChanged;

			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUILeft);
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUIRight);
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
		}

		public static void OnToolbarGUILeft()
		{
			//下面只是Demo，可随便更改
			GUILayout.FlexibleSpace();
			string finalStr = $"AppName : {ProjectApp.AppFacade.AppDesc}";

			GUILayout.Label(finalStr, new GUIStyle("SelectionRect"));

			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Play MainScene", EditorGUIUtility.FindTexture("PlayButton"), "跳转主场景并播放")))
			{
				EditorSceneTool.PlayMainScene();
            }
			
			GUILayout.Space(30);
		}

		public static void OnToolbarGUIRight()
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