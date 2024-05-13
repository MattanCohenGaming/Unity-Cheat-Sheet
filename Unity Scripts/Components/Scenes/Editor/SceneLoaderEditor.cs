using System.Collections.Generic;
using Newledge.Core.Utils.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MCG.UnityCheatSheet.Editor
{
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SceneLoader loader = (SceneLoader)target;

            GUILayout.Space(40);

            DrawSceneChoices(loader);

            if (string.IsNullOrEmpty(loader.sceneToLoadName))
                return;

            GUILayout.Space(40f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Scene"))
            {
                loader.LoadScene();
            }
            if (loader.IsDesiredSceneLoaded)
                if (GUILayout.Button("Start Loaded Scene"))
                {
                    loader.LoadScene();
                    loader.StartScene();
                }
            GUILayout.EndHorizontal();
        }

        private string SceneNameFromBuildScene(EditorBuildSettingsScene scene) =>
            System.IO.Path.GetFileNameWithoutExtension(scene.path);

        private void DrawSceneChoices(SceneLoader loader)
        {
            // Get active scene list from build settings
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length == 0)
            {
                Debug.LogWarning("No scenes found in Build Settings.");
                return;
            }

            List<string> sceneNames = new List<string>();
            for (int i = 0; i < buildScenes.Length; i++)
            {
                var buildSceneName = SceneNameFromBuildScene(buildScenes[i]);

                sceneNames.Add(buildSceneName);
            }

            // Find current scene index based on last selection or default to 0
            int currentSceneIndex = System
                .Array
                .IndexOf(sceneNames.ToArray(), loader.sceneToLoadName);

            if (currentSceneIndex == -1)
            {
                currentSceneIndex = 0;
            }

            var sceneNamesWithNumbers = new List<string>();
            for (int sceneIndex = 0; sceneIndex < sceneNames.Count; sceneIndex++)
            {
                var sceneName = sceneNames[sceneIndex];
                sceneNamesWithNumbers.Add((sceneIndex + 1) + ") " + sceneName);
            }

            // Dropdown for selecting scene
            int selectedSceneIndex = EditorGUILayout.Popup(
                "Scene To Load",
                currentSceneIndex,
                sceneNamesWithNumbers.ToArray()
            );
            if (selectedSceneIndex >= 0 && selectedSceneIndex < sceneNames.Count)
            {
                PickScene(sceneNames[selectedSceneIndex], loader);
            }
        }

        private void PickScene(string sceneName, SceneLoader loader)
        {
            if (GUI.changed)
            {
                loader.sceneToLoadName = sceneName;

                // Explicitly mark the scene as dirty
                if (!EditorApplication.isPlaying)
                {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }
    }
}
