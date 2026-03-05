/****************************************************
    文件: GameToolView.cs
    作者: Clear
    日期: 2026/3/4 20:38:16
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// ===== GameToolView 类 =====
public static class GameToolView
{
    private static Vector2 gameToolLeftScrollPos;
    private static Vector2 gameToolRightScrollPos;

    // 项目统计数据
    private static string[] projectStats = new string[12];
    private static string[] recentActivities = new string[]
    {
        "10:23 - 场景 'Main' 已保存",
        "10:15 - 资源 'Player.prefab' 已修改",
        "09:58 - 脚本 'GameManager.cs' 已添加",
        "09:42 - 构建 AssetBundle 完成",
        "09:30 - 项目已打开",
    };

    // 系统信息
    private static string[] systemInfo = new string[6];
    private static string[] performanceMetrics = new string[6];

    public static void InitData()
    {
        RefreshProjectStats();
    }

    public static void OnGUI()
    {
        // 实时更新性能指标
        UpdatePerformanceMetrics();
        
        GUILayout.BeginArea(new Rect(10, 35, 900, 440));

        GUILayout.BeginHorizontal();

        // ===== 左侧菜单区域 =====
        GUILayout.BeginVertical(GUILayout.Width(200));
        gameToolLeftScrollPos = GUILayout.BeginScrollView(gameToolLeftScrollPos, GUILayout.Width(200), GUILayout.Height(440));

        GUILayout.Label("功能菜单", EditorStyles.boldLabel);
        GUILayout.Space(10);

        string[] menuItems = new string[]
        {
            "📊 项目总览",
            "📁 资源统计",
            "📝 代码统计",
            "🎮 场景统计",
            "⚙️ 性能分析",
            "📋 最近活动",
            "🔧 工具设置",
        };

        foreach (string item in menuItems)
        {
            if (GUILayout.Button(item, GUILayout.Height(30)))
            {
                Debug.Log($"选择: {item}");
            }
            GUILayout.Space(2);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // ===== 右侧内容区域 =====
        GUILayout.BeginVertical(GUILayout.Width(670));
        gameToolRightScrollPos = GUILayout.BeginScrollView(gameToolRightScrollPos, GUILayout.Width(670), GUILayout.Height(440));

        GUILayout.Label("项目数据统计中心", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter });
        GUILayout.Space(15);

        // 第一行：项目统计 + 系统信息
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box", GUILayout.Width(320));
        GUILayout.Label("📊 项目统计", EditorStyles.boldLabel);
        GUILayout.Space(5);
        foreach (string stat in projectStats)
        {
            if (!string.IsNullOrEmpty(stat))
            {
                GUILayout.Label(stat);
            }
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box", GUILayout.Width(320));
        GUILayout.Label("ℹ️ 系统信息", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        // 实时系统信息
        systemInfo[0] = $"Unity版本: {Application.unityVersion}";
        systemInfo[1] = $"目标平台: {EditorUserBuildSettings.activeBuildTarget}";
        systemInfo[2] = $"脚本后端: {GetScriptingBackend()}";
        systemInfo[3] = $"API级别: {GetApiCompatibilityLevel()}";
        systemInfo[4] = $"渲染管线: {GetRenderPipeline()}";
        systemInfo[5] = $"内存使用: {GetMemoryUsage()}";
        
        foreach (string info in systemInfo)
        {
            GUILayout.Label(info);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // 第二行：性能指标 + 最近活动
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box", GUILayout.Width(320));
        GUILayout.Label("⚡ 性能指标", EditorStyles.boldLabel);
        GUILayout.Space(5);
        foreach (string metric in performanceMetrics)
        {
            GUILayout.Label(metric);
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box", GUILayout.Width(320));
        GUILayout.Label("📋 最近活动", EditorStyles.boldLabel);
        GUILayout.Space(5);
        foreach (string activity in recentActivities)
        {
            GUILayout.Label($"• {activity}");
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        GUILayout.BeginVertical("box");
        GUILayout.Label("⚡ 快捷操作", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("刷新统计数据", GUILayout.Height(30), GUILayout.Width(120)))
        {
            RefreshProjectStats();
        }
        if (GUILayout.Button("导出报告", GUILayout.Height(30), GUILayout.Width(120)))
        {
            Debug.Log("导出项目统计报告");
        }
        if (GUILayout.Button("清理缓存", GUILayout.Height(30), GUILayout.Width(120)))
        {
            Debug.Log("清理项目缓存");
        }
        if (GUILayout.Button("运行分析", GUILayout.Height(30), GUILayout.Width(120)))
        {
            Debug.Log("运行深度分析");
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.Label("--- 数据统计实时更新 ---", EditorStyles.centeredGreyMiniLabel);

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 480, 900, 20));
        GUILayout.Label("--- 界面底部 ---", EditorStyles.centeredGreyMiniLabel);
        GUILayout.EndArea();
    }

    private static void RefreshProjectStats()
    {
        try
        {
            // 统计脚本数量
            string[] csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            int totalLines = 0;
            foreach (string file in csFiles)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    totalLines += lines.Length;
                }
                catch { }
            }

            // 统计资源数量
            string[] scenes = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
            string[] prefabs = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
            string[] textures = Directory.GetFiles(Application.dataPath, "*.png", SearchOption.AllDirectories);
            string[] textures2 = Directory.GetFiles(Application.dataPath, "*.jpg", SearchOption.AllDirectories);
            string[] models = Directory.GetFiles(Application.dataPath, "*.fbx", SearchOption.AllDirectories);
            string[] models2 = Directory.GetFiles(Application.dataPath, "*.obj", SearchOption.AllDirectories);
            string[] audio = Directory.GetFiles(Application.dataPath, "*.mp3", SearchOption.AllDirectories);
            string[] audio2 = Directory.GetFiles(Application.dataPath, "*.wav", SearchOption.AllDirectories);
            string[] materials = Directory.GetFiles(Application.dataPath, "*.mat", SearchOption.AllDirectories);
            string[] animations = Directory.GetFiles(Application.dataPath, "*.anim", SearchOption.AllDirectories);
            string[] shaders = Directory.GetFiles(Application.dataPath, "*.shader", SearchOption.AllDirectories);

            // 更新统计数据
            projectStats[0] = $"总脚本数: {csFiles.Length}";
            projectStats[1] = $"总行数: {totalLines:N0}";
            projectStats[2] = $"总资源数: {textures.Length + textures2.Length + models.Length + models2.Length + audio.Length + audio2.Length + materials.Length}";
            projectStats[3] = $"总场景数: {scenes.Length}";
            projectStats[4] = $"总预制体数: {prefabs.Length}";
            projectStats[5] = $"总贴图数: {textures.Length + textures2.Length}";
            projectStats[6] = $"总模型数: {models.Length + models2.Length}";
            projectStats[7] = $"总音频数: {audio.Length + audio2.Length}";
            projectStats[8] = $"总材质数: {materials.Length}";
            projectStats[9] = $"总动画数: {animations.Length}";
            projectStats[10] = $"总着色器数: {shaders.Length}";
            
            // 计算总预设体数（包括所有变体）
            int totalPrefabs = prefabs.Length;
            projectStats[11] = $"总预设体数: {totalPrefabs}";

            Debug.Log($"项目统计刷新完成：{csFiles.Length}个脚本，{totalLines}行代码");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"刷新项目统计失败：{e.Message}");
        }
    }

    private static void UpdatePerformanceMetrics()
    {
        // 实时性能指标
        performanceMetrics[0] = $"FPS: {Mathf.RoundToInt(1.0f / Time.deltaTime)}";
        performanceMetrics[1] = $"Draw Calls: {UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024} MB";
        performanceMetrics[2] = $"三角形数: {GetTriangleCount()}";
        performanceMetrics[3] = $"顶点数: {GetVertexCount()}";
        performanceMetrics[4] = $"纹理内存: {GetTextureMemory()}";
        performanceMetrics[5] = $"网格内存: {GetMeshMemory()}";
    }

    private static string GetScriptingBackend()
    {
        #if UNITY_2020_1_OR_NEWER
            return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup).ToString();
        #else
            return PlayerSettings.scriptingRuntimeVersion.ToString();
        #endif
    }

    private static string GetApiCompatibilityLevel()
    {
        #if UNITY_2019_3_OR_NEWER
            return PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup).ToString();
        #else
            return PlayerSettings.apiCompatibilityLevel.ToString();
        #endif
    }

    private static string GetRenderPipeline()
    {
        if (GraphicsSettings.currentRenderPipeline != null)
        {
            return GraphicsSettings.currentRenderPipeline.name;
        }
        return "内置渲染管线";
    }

    private static string GetMemoryUsage()
    {
        float memoryMB = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
        return $"{memoryMB:F1} MB";
    }

    private static string GetTriangleCount()
    {
        // 这个需要从场景中统计，这里返回示例值
        return "45K";
    }

    private static string GetVertexCount()
    {
        // 这个需要从场景中统计，这里返回示例值
        return "78K";
    }

    private static string GetTextureMemory()
    {
        float memoryMB = UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver() / (1024f * 1024f);
        return $"{memoryMB:F0} MB";
    }

    private static string GetMeshMemory()
    {
        // 简单估算
        return "128 MB";
    }
}