/****************************************************
    文件: CommonInfoView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: 通用信息显示类
*****************************************************/
using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    public static class CommonInfoView
    {
        private static Vector2 infoScrollPos;
        private static string searchFilter = "";

        private static string[] logMessages = new string[]
        {
            "[INFO] 系统初始化完成 - 2024/1/15 10:23:45",
            "[INFO] 资源加载完成 - 2024/1/15 10:23:46",
            "[WARNING] 配置文件缺失，使用默认配置 - 2024/1/15 10:23:47",
            "[ERROR] 网络连接超时，重试中... - 2024/1/15 10:23:48",
            "[INFO] 重新连接成功 - 2024/1/15 10:23:50",
            "[DEBUG] 玩家数据加载: 1000ms - 2024/1/15 10:23:51",
            "[INFO] 场景切换完成 - 2024/1/15 10:23:52",
            "[WARNING] 材质丢失，使用默认材质 - 2024/1/15 10:23:53",
            "[INFO] 音频系统初始化 - 2024/1/15 10:23:54",
            "[INFO] 输入系统初始化 - 2024/1/15 10:23:55",
            "[ERROR] 着色器编译失败 - 2024/1/15 10:23:56",
            "[INFO] 重新编译着色器 - 2024/1/15 10:23:57",
            "[INFO] 编译成功 - 2024/1/15 10:23:58",
        };

        public static void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(650));

            // 工具栏
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("🔍 搜索:", GUILayout.Width(50));
            searchFilter = GUILayout.TextField(searchFilter, EditorStyles.toolbarTextField, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刷新", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                Debug.Log("刷新信息");
            }
            if (GUILayout.Button("清空日志", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                System.Array.Clear(logMessages, 0, logMessages.Length);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 日志显示区域
            GUILayout.BeginVertical("box");
            GUILayout.Label("📋 实时日志", EditorStyles.boldLabel);
            GUILayout.Space(5);

            infoScrollPos = GUILayout.BeginScrollView(infoScrollPos, GUILayout.Height(400));

            foreach (string log in logMessages)
            {
                if (string.IsNullOrEmpty(searchFilter) || log.ToLower().Contains(searchFilter.ToLower()))
                {
                    GUILayout.BeginHorizontal();

                    // 根据日志级别显示不同颜色的圆点
                    if (log.Contains("[ERROR]"))
                    {
                        GUI.color = Color.red;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }
                    else if (log.Contains("[WARNING]"))
                    {
                        GUI.color = Color.yellow;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }
                    else if (log.Contains("[INFO]"))
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }
                    else
                    {
                        GUI.color = Color.gray;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }

                    GUI.color = Color.white;
                    GUILayout.Label(log);

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
    }
}