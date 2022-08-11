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
			//����ֻ��Demo����������
			GUILayout.Space(100);
			string finalStr = $"AppName : {ProjectApp.AppFacade.AppDesc}";

			GUILayout.Label(finalStr, new GUIStyle("SelectionRect"));

			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Play MainScene", EditorGUIUtility.FindTexture("PlayButton"), "��ת������������")))
			{
				EditorSceneTool.PlayMainScene();
            }
			
			GUILayout.Space(30);
		}

		static void OnToolbarGUIRight()
		{
			//����ֻ��Demo����������
			GUILayout.Space(1);
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