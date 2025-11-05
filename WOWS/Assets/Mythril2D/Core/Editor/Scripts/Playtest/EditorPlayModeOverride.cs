using Gyvr.Mythril2D;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gyvr.Mythril2D
{
    [InitializeOnLoad]
    public class EditorPlayModeOverride
    {
        // Player Prefs key names
        const string kEditorOpenedScene = "PreviousScenePath";
        const string kPlaytestMode = "EditorIntialization";

        private static bool IsEditingMap()
        {
            return Object.FindAnyObjectByType<MapInfo>();
        }

        static EditorPlayModeOverride()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static string FindScenePathFromName(string targetSceneName)
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);

                if (sceneName == targetSceneName)
                {
                    return scene.path;
                }
            }

            return string.Empty;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (IsEditingMap())
                {
                    EditorPrefs.SetString(kEditorOpenedScene, activeScene.name);
                    EditorPrefs.SetBool(kPlaytestMode, true);
                    SetStartScene(FindScenePathFromName(Constants.M2DEngineSceneName));
                }
                else
                {
                    EditorPrefs.SetString(kEditorOpenedScene, activeScene.name);
                    EditorPrefs.SetBool(kPlaytestMode, false);
                    EditorSceneManager.playModeStartScene = null;
                }
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                string editorOpenedScene = EditorPrefs.GetString(EditorPlayModeOverride.kEditorOpenedScene);

                if (EditorPrefs.GetBool(kPlaytestMode))
                {
                    LoadMap();
                }

                // Workaround to avoid permenant black screen in gameplay scene playtest,
                // and to show the UI
                if (editorOpenedScene == Constants.M2DEngineSceneName)
                {
                    GameManager.TransitionSystem.gameObject.SetActive(false);
                    GameManager.UISystem.ShowUI();
                }
            }
        }

        private static void SetStartScene(string scenePath)
        {
            SceneAsset firstSceneToLoad = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (firstSceneToLoad != null)
            {
                EditorSceneManager.playModeStartScene = firstSceneToLoad;
            }
            else
            {
                Debug.LogError("Could not find Scene " + scenePath);
            }
        }

        private static void LoadMap()
        {
            var prevScene = EditorPrefs.GetString(kEditorOpenedScene);

            SaveDataBlock saveData = GameManager.SaveSystem.DuplicateSaveData(GameManager.Config.playtestSaveFile.content);

            saveData.map = new MapDataBlock
            {
                checkpoints = new ICheckpoint[0],
                currentMap = prevScene,
                playtest = true
            };

            GameManager.SaveSystem.LoadDataBlock(saveData);
        }
    }
}
