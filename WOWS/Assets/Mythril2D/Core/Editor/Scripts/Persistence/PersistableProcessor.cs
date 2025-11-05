using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gyvr.Mythril2D
{
    // This class ensures that all Persistable objects in the scene have a unique identifier
    // as soon as they are created. This is necessary for the PersistenceSystem to work correctly.
    [InitializeOnLoad]
    static class PersistableProcessor
    {
        private static HashSet<int> m_persistables = new();

        static PersistableProcessor()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
        }

        private static IEnumerable<Persistable> FindPersistables() => Object.FindObjectsByType<Persistable>(FindObjectsSortMode.InstanceID);

        private static void CacheInitialPersistables()
        {
            m_persistables = FindPersistables().Select(p => p.GetInstanceID()).ToHashSet();
        }

        private static void FixInvalidIdentifiers()
        {
            if (!Application.isPlaying)
            {
                IEnumerable<Persistable> invalidPersistables = FindPersistables().Where(p => PersistanceUtil.IsMissingIdentifier(p));

                if (invalidPersistables.Any())
                {
                    EditorUtility.DisplayDialog(
                        "Mythril2D Persistence System: Missing Identifiers",
                        $"Found {invalidPersistables.Count()} persistable(s) with missing identifiers.\n\n" +
                        "Each persistable (object that can potentially be saved) requires a unique identifier for the save system to function correctly. Please generate a new identifier for each one.",
                        $"Generate {invalidPersistables.Count()} Identifiers"
                    );

                    foreach (Persistable persistable in invalidPersistables)
                    {
                        PersistanceUtil.GenerateIdentifierFor(persistable, "Fixed");
                        m_persistables.Add(persistable.GetInstanceID());
                    }
                }
            }
        }

        private static void OnActiveSceneChanged(Scene previous, Scene current)
        {
            if (!Application.isPlaying)
            {
                CacheInitialPersistables();
            }
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!Application.isPlaying)
            {
                CacheInitialPersistables();
            }
        }

        private static void OnHierarchyChanged()
        {
            if (!Application.isPlaying)
            {
                foreach (Persistable persistable in FindPersistables())
                {
                    if (m_persistables.Add(persistable.GetInstanceID()))
                    {
                        PersistanceUtil.GenerateIdentifierFor(persistable);
                    }
                }

                // Last resort to ensure all Persistables have an identifier
                FixInvalidIdentifiers();
            }
        }
    }
}
