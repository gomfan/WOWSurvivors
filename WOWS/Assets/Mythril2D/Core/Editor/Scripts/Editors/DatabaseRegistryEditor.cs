using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CustomEditor(typeof(DatabaseRegistry))]
    public class DatabaseRegistryEditor : FancyEditor<DatabaseRegistry>
    {
        private struct RegistryInfo
        {
            public int entryCount;
            public int missingReferenceCount;
            public int conversionCount;
        }

        protected override bool m_drawScriptHeader => false;

        private RegistryInfo CollectRegistryInfo(DatabaseRegistry registry)
        {
            RegistryInfo info = new()
            {
                entryCount = registry.entries.Count,
                missingReferenceCount = 0,
                conversionCount = registry.GUIDConversionMap.Count
            };

            foreach (var entry in registry.entries)
            {
                if (entry.Value == null)
                {
                    ++info.missingReferenceCount;
                }
            }

            return info;
        }

        // Draw the summary (entry count, missing reference count, etc... in a help box)
        private void DrawSummary(RegistryInfo info)
        {
            EditorGUILayout.HelpBox($"Entries: {info.entryCount}\nConversions: {info.conversionCount}\nMissing References: {info.missingReferenceCount}", MessageType.Info);
        }

        protected override void DrawCustomInspector(DatabaseRegistry registry)
        {
            RegistryInfo info = CollectRegistryInfo(registry);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Regenerate"))
            {
                // Show a popup message to confirm the action
                bool confirmed = info.entryCount == 0 || EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to replace the database registry content with all database entries? The current content of the database will be overwritten.", "Regenerate", "Cancel");

                if (confirmed)
                {
                    registry.Set(Resources.FindObjectsOfTypeAll<DatabaseEntry>());
                }
            }

            if (GUILayout.Button("Add Missing Entries"))
            {
                var entries = Resources.FindObjectsOfTypeAll<DatabaseEntry>().Where(entry => !registry.HasGUID(entry.GetAssetGUID())).ToArray();
                
                foreach (var entry in entries)
                {
                    registry.Register(entry);
                }
            }

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = info.entryCount > 0;
            if (GUILayout.Button("Clear All Entries"))
            {
                bool confirmed = EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to clear the database registry? This action cannot be undone.", "Clear All Entries", "Cancel");

                if (confirmed)
                {
                    registry.Clear();
                }
            }
            GUI.enabled = wasGUIEnabled;

            wasGUIEnabled = GUI.enabled;
            GUI.enabled = info.missingReferenceCount > 0;
            if (GUILayout.Button("Remove Missing References"))
            {
                registry.RemoveMissingReferences();
            }
            GUI.enabled = wasGUIEnabled;

            GUILayout.EndHorizontal();

            DrawSummary(info);
        }
    }
}
