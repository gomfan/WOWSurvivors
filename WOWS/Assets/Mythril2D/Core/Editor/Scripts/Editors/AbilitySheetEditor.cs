using System;
using UnityEditor;

namespace Gyvr.Mythril2D
{
    [CustomEditor(typeof(AbilitySheet), true)]
    public class AbilitySheetEditor : DatabaseEntryEditor
    {
        private void ShowMessage(MessageType type, string message, params object[] args)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(string.Format(message, args), type);
        }

        static Type FindGenericBaseTemplateParameter(Type type, Type genericBase)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBase.GetGenericTypeDefinition())
            {
                return type.GetGenericArguments()[0];
            }
            else if (type.BaseType != null)
            {
                return FindGenericBaseTemplateParameter(type.BaseType, genericBase);
            }
            else
            {
                return null;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AbilitySheet abilitySheet = target as AbilitySheet;

            if (abilitySheet.prefab)
            {
                var abilityBase = abilitySheet.prefab.GetComponent<AbilityBase>();

                if (abilityBase == null)
                {
                    ShowMessage(MessageType.Error, "The provided ability prefab must have a component of type {0} attached to its root!", nameof(AbilityBase));
                }
                else
                {
                    Type abilitySheetType = abilitySheet.GetType();
                    Type abilityExpectedSheetType = FindGenericBaseTemplateParameter(abilityBase.GetType(), typeof(Ability<>));

                    if (abilitySheetType != abilityExpectedSheetType)
                    {
                        if (!abilitySheetType.IsSubclassOf(abilityExpectedSheetType))
                        {
                            ShowMessage(MessageType.Error, "\"{0}\" is incompatible with {1}. A ScriptableObject of type {2} is required to work with this prefab.",
                                abilitySheet.prefab.name,
                                abilitySheetType.Name,
                                abilityExpectedSheetType.Name);
                        }
                    }
                }
            }
        }
    }
}
