using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Gyvr.Mythril2D
{
    public static class SceneUtil
    {
        public static IEnumerable<string> GetAllScenesInBuildSettings()
        {
            List<string> sceneNames = new();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                sceneNames.Add(sceneName);
            }

            return sceneNames;
        }

        public static IEnumerable<string> GetAllScenesInAssetDatabase()
        {
            string[] guids = AssetDatabase.FindAssets("t:scene", null);
            return guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
