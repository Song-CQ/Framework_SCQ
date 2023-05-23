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
			//����ֻ��Demo����������
			GUILayout.FlexibleSpace();
			string finalStr = $"AppName : {ProjectApp.AppFacade.AppDesc}";

			GUILayout.Label(finalStr, new GUIStyle("SelectionRect"));

			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Play MainScene", EditorGUIUtility.FindTexture("PlayButton"), "��ת������������")))
			{
				AssetDatabase.Refresh();
				EditorSceneTool.PlayMainScene();
            }
			
			GUILayout.Space(30);
		}

		public static void OnToolbarGUIRight()
		{
			//����ֻ��Demo����������
			if (GUILayout.Button(new GUIContent(EditorGUIUtility.FindTexture("d_RotateTool On"), "ˢ�±������"), /*ToolbarStyles.ToolBarExtenderBtnStyle,*/GUILayout.Width(30)))
			{
				AssetDatabase.Refresh();
				Debug.Log("TODO :ˢ�����");
			}
			GUILayout.Space(10);
			
			if (GUILayout.Button(new GUIContent("Test Scene", EditorGUIUtility.FindTexture("Button"), "��ת���Գ���")))
			{
				EditorSceneTool.LoadTestScene();
			}

			if (GUILayout.Button(new GUIContent("Open Framework View", EditorGUIUtility.FindTexture("Button"), "�򿪹��߽���")))
			{
				FrameworkToolView.OpenFrameworkWin();
			}

			GUILayout.FlexibleSpace();

			

			//if (GUILayout.Button(new GUIContent("SVN����", "���µ�ǰ�Ŀͻ���"), ToolbarStyles.ToolBarExtenderBtnStyle))
			//{
			//	SVNUtils.UpdateSVNProject();

			//	Debug.Log("TODO : SVN����");
			//}
			//if (GUILayout.Button(new GUIContent("SVN�ύ", "�ύ�ͻ���"), ToolbarStyles.ToolBarExtenderBtnStyle))
			//{
			//	SVNUtils.UpdateCommitProject();
			//	Debug.Log("TODO : SVN�ύ");
			//}
			//Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 10, new GUIStyle("MiniSliderHorizontal"), new GUIStyle("MinMaxHorizontalSliderThumb"), GUILayout.MinWidth(200), GUILayout.MinHeight(20));
		}
	}

	

}