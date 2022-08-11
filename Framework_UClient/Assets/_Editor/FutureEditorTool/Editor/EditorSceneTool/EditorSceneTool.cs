using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FutureEditor
{
    public static class EditorSceneTool
    {
        public const string MainScene = "Assets/_AppBase/Scene/0_MainScene.unity";
        public const string TestScene = "Assets/_Editor/EditorScene/0_TestScene.unity";

        public static void PlayMainScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorApplication.isPlaying = false;

                EditorCoroutineRunner.StartEditorCoroutine(playScene(() =>
                {
                    EditorSceneManager.OpenScene(MainScene);
                    EditorApplication.isPlaying = true;
                }));
            }

        }
        public static void LoadTestScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorApplication.isPlaying = false;

                EditorCoroutineRunner.StartEditorCoroutine(playScene(() =>
                {
                    EditorSceneManager.OpenScene(TestScene);
                }));
            }
        }

        private static IEnumerator playScene(Action cb)
        {
            while (Application.isPlaying)
                yield return null;

            cb();
        }
    }
}