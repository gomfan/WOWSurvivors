using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Gyvr.Mythril2D
{
    [Overlay(typeof(SceneView), "Mythril2D Scene Selector", true)]
    public class SceneSelectorOverlay : Overlay
    {
        public SceneSelectorOverlay()
        {
            EditorApplication.projectChanged += OnProjectChanged;
        }

        private void Repaint()
        {
            if (displayed)
            {
                displayed = false;
                displayed = true;
            }
        }

        private void OnProjectChanged() => Repaint();

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new();
            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.maxHeight = 250;

            foreach (var scene in SceneUtil.GetAllScenesInAssetDatabase())
            {
                Button button = new()
                {
                    text = Path.GetFileNameWithoutExtension(scene)
                };

                button.clicked += () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scene);
                    }
                };

                scrollView.Add(button);
            }

            root.Add(scrollView);
            return root;
        }
    }
}
