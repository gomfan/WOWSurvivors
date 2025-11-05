using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class ScriptableObjectPostprocessor : AssetPostprocessor
    {
        public static List<ScriptableObject> cachedScriptableObjects => _cachedScriptableObjects;

        private static List<ScriptableObject> _cachedScriptableObjects = new();

        static ScriptableObjectPostprocessor()
        {
            UpdateScriptableObjectCache();
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UpdateScriptableObjectCache();
        }

        private static void UpdateScriptableObjectCache()
        {
            _cachedScriptableObjects.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject instance = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (instance != null)
                {
                    _cachedScriptableObjects.Add(instance);
                }
            }
        }
    }

    public class DatabaseWindow : EditorWindow
    {
        private static readonly Type[] Tabs = {
            typeof(HeroSheet),
            typeof(MonsterSheet),
            typeof(NPCSheet),
            typeof(AbilitySheet),
            typeof(Item),
            typeof(Shop),
            typeof(Recipe),
            typeof(CraftingStation),
            typeof(Inn),
            typeof(Quest),
            typeof(QuestTask),
            typeof(DialogueSequence),
            typeof(AudioClipResolver),
            typeof(SaveFile),
            typeof(PrefabReference),
            typeof(CommandHandler),
            typeof(NavigationCursorStyle),
            typeof(GameConfig),
            typeof(DatabaseRegistry)
        };

        private static int _selectedTab = 0;
        private static int _selectedIndex = -1;
        private static Vector2 _scrollPos;
        private static string _searchString = string.Empty;

        [MenuItem("Window/Mythril2D/Database")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<DatabaseWindow>();
            window.titleContent = new GUIContent("Database");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            var previousSelectedTab = _selectedTab;
            _selectedTab = GUILayout.SelectionGrid(_selectedTab, Tabs.Select(t => t.Name).ToArray(), 1);

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // Search field to filter elements
            var previousSearchString = _searchString;
            _searchString = GUILayout.TextField(_searchString, EditorStyles.toolbarSearchField);

            if (previousSearchString != _searchString || _selectedTab != previousSelectedTab)
            {
                _selectedIndex = -1;
            }

            // Create an array of scriptable object names that match the selected tab
            var visibleScriptableObjects = ScriptableObjectPostprocessor.cachedScriptableObjects
                .Where(so => Tabs[_selectedTab].IsAssignableFrom(so.GetType()) && so.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(so => so.name);

            var names = visibleScriptableObjects.Select(so => so.name).ToArray();

            // Use a SelectionGrid to display the names and get the selected index
            var previouslySelectedIndex = _selectedIndex;
            _selectedIndex = GUILayout.SelectionGrid(_selectedIndex, names, 1, EditorStyles.objectField);

            // If an index is selected, set the active object to the corresponding scriptable object
            if (_selectedIndex >= 0 && previouslySelectedIndex != _selectedIndex)
            {
                Selection.activeObject = visibleScriptableObjects.First(so => so.name == names[_selectedIndex]);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}