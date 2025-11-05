using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Gyvr.Mythril2D
{
    public static class DatabaseRegistryExtensions
    {
        public static bool HasGUID(this DatabaseRegistry registry, string guid)
        {
            return registry.entries.Contains(guid);
        }

        public static bool HasGUIDConversion(this DatabaseRegistry registry, string guid)
        {
            return registry.GUIDConversionMap.Contains(guid);
        }

        public static void Set(this DatabaseRegistry registry, DatabaseEntry[] entries)
        {
            registry.Initialize(entries.ToDictionary(entry => entry.GetAssetGUID(), entry => entry));
            registry.ForceSave();
        }

        public static void Register(this DatabaseRegistry registry, DatabaseEntry entry)
        {
            registry.entries.TryAdd(entry.GetAssetGUID(), entry);
            registry.ForceSave();
        }

        public static void Unregister(this DatabaseRegistry registry, DatabaseEntry entry)
        {
            registry.RemoveAt(entry.GetAssetGUID());
        }

        public static void RemoveAt(this DatabaseRegistry registry, string guid)
        {
            registry.entries.Remove(guid);
            registry.ForceSave();
        }

        public static void RemoveMissingReferences(this DatabaseRegistry registry)
        {
            List<string> keysToRemove = new();

            foreach (var entry in registry.entries)
            {
                if (entry.Value == null)
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                registry.entries.Remove(key);
            }

            registry.ForceSave();
        }

        public static void Clear(this DatabaseRegistry registry)
        {
            registry.entries.Clear();
            registry.ForceSave();
        }

        public static void RemoveConversion(this DatabaseRegistry registry, string from)
        {
            registry.GUIDConversionMap.Remove(from);
            registry.ForceSave();
        }

        public static void SetConversion(this DatabaseRegistry registry, string from, string to)
        {
            registry.GUIDConversionMap[from] = to;
            registry.ForceSave();
        }

        private static void ForceSave(this DatabaseRegistry registry)
        {
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssetIfDirty(registry);
        }
    }
}
