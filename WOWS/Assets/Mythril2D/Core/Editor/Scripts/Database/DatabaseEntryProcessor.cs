using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Gyvr.Mythril2D
{
    public class DatabaseEntryProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            DatabaseRegistry[] registries = AssetLoader.LoadAssetsOfType<DatabaseRegistry>();

            HandleImportedAssets(
                registries.Where(r => r.autoAddNewDatabaseEntries),
                importedAssets
            );

            HandleDeletedAssets(
                registries.Where(r => r.autoRemoveDatabaseEntries),
                deletedAssets
            );
        }

        private static void HandleImportedAssets(IEnumerable<DatabaseRegistry> registries, string[] paths)
        {
            foreach (string path in paths)
            {
                DatabaseEntry entry = AssetDatabase.LoadAssetAtPath<DatabaseEntry>(path);

                if (entry != null)
                {
                    foreach (DatabaseRegistry registry in registries)
                    {
                        registry.Register(entry);
                    }
                }
            }
        }

        private static void HandleDeletedAssets(IEnumerable<DatabaseRegistry> registries, string[] paths)
        {
            foreach (string path in paths)
            {
                string guid = AssetDatabase.AssetPathToGUID(path);

                if (!string.IsNullOrEmpty(guid))
                {
                    foreach (DatabaseRegistry registry in registries)
                    {
                        registry.RemoveAt(guid);
                    }
                }
            }
        }
    }
}
