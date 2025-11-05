using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public static class AssetLoader
    {
        public static T[] LoadAssetsOfType<T>() where T : Object
        {
            return AssetDatabase
                .FindAssets($"t:{typeof(T).Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToArray();
        }
    }
}