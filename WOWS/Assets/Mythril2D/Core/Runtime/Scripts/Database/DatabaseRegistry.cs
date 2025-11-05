using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D + nameof(DatabaseRegistry))]
    public class DatabaseRegistry : ScriptableObject
    {
        public bool autoAddNewDatabaseEntries => m_autoAddNewDatabaseEntries;
        public bool autoRemoveDatabaseEntries => m_autoRemoveDatabaseEntries;
        public SerializableDictionary<string, DatabaseEntry> entries => m_entries;
        public SerializableDictionary<string, string> GUIDConversionMap => m_GUIDConversionMap;

        [Header("Automation Settings")]
        [SerializeField] private bool m_autoAddNewDatabaseEntries = true;
        [SerializeField] private bool m_autoRemoveDatabaseEntries = true;

        [Header("Database Content")]
        [SerializeField] private SerializableDictionary<string, DatabaseEntry> m_entries = null;
        [SerializeField] private SerializableDictionary<string, string> m_GUIDConversionMap = null;

        public DatabaseEntryReference<T> CreateReference<T>(T entry) where T : DatabaseEntry
        {
            string guid = DatabaseEntryToGUID(entry);
            return new(guid);
        }

        public T LoadFromReference<T>(DatabaseEntryReference<T> reference) where T : DatabaseEntry
        {
            return GUIDToDatabaseEntry<T>(reference.guid);
        }

        public T GUIDToDatabaseEntry<T>(string guid) where T : DatabaseEntry
        {
            HashSet<string> visited = new();

            // Convert the GUID if it exists in the conversion map
            while (m_GUIDConversionMap.ContainsKey(guid))
            {
                guid = m_GUIDConversionMap[guid];
                if (visited.Contains(guid))
                {
                    Debug.LogError($"Circular reference detected in DatabaseRegistry: {guid}");
                    return null;
                }
                visited.Add(guid);
            }

            return m_entries.TryGetValue(guid, out DatabaseEntry entry) ? entry as T : null;
        }

        public string DatabaseEntryToGUID<T>(T instance) where T : DatabaseEntry
        {
            string guid = m_entries.FirstOrDefault(entry => entry.Value == instance).Key;
            Debug.Assert(!string.IsNullOrEmpty(guid), $"Database entry {instance} does not exist in the registry.");
            return guid;
        }

        public void Initialize(Dictionary<string, DatabaseEntry> data)
        {
            m_entries = new SerializableDictionary<string, DatabaseEntry>(data);
        }
    }
}
