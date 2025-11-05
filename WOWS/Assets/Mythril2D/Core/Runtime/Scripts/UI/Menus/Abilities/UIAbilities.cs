using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    public enum EAbilityType
    {
        Passive,
        Active
    }

    public class UIAbilities : AUIMenu
    {
        [Header("References")]
        [SerializeField] private GameObject m_abilityBarEntryPrefab = null;
        [SerializeField] private GameObject m_abilityListRoot = null;
        [SerializeField] private CanvasGroup m_listCanvasGroup = null;
        [SerializeField] private TextMeshProUGUI m_description = null;
        [SerializeField] private UIAbilityBar m_abilityBar = null;
        [SerializeField] private SerializableDictionary<EAbilityType, UIAbilityCategory> m_categories = null;
        [SerializeField] private List<GameObject> m_toEnableWhenEquippingAnAbility = null;

        [Header("Settings")]
        [SerializeField][TextArea] private string m_activeAbilityDescription;
        [SerializeField][TextArea] private string m_passiveAbilityDescription;

        private UIAbilityListEntry[] m_entries = null;
        private UIAbilityListEntry m_abilitySelected = null;

        public override bool OnCancel()
        {
            if (m_abilitySelected != null)
            {
                ExitEquipMode(m_abilitySelected);
                return true;
            }

            return false;
        }

        private int GetAbilityCount(EAbilityType type)
        {
            return GameManager.Player.abilityInstances.Count(ability =>
                type == EAbilityType.Active ?
                ability is ActiveAbilityBase :
                ability is PassiveAbilityBase
            );
        }

        public override GameObject FindSomethingToSelect()
        {
            if (m_entries.Length > 0)
            {
                return m_entries[0].gameObject;
            }

            return base.FindSomethingToSelect();
        }

        protected override void OnMenuShown(params object[] args)
        {
            Debug.Assert(args.Length == 0, "Abilities menu invoked with incorrect arguments");
            m_abilityBar.Init();
            m_abilityBar.UpdateUI();
            SelectCategory(EAbilityType.Active);
            UpdateUI();
        }

        override protected void OnMenuHidden()
        {
            ExitEquipMode();
        }

        private void UpdateUI()
        {
            foreach (var category in m_categories)
            {
                category.Value.SetCategory(category.Key, GetAbilityCount(category.Key));
            }
        }

        private void ClearSpellBookList()
        {
            for (int i = 0; i < m_abilityListRoot.transform.childCount; ++i)
            {
                Transform child = m_abilityListRoot.transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        private void FillSpellBookList(EAbilityType type = EAbilityType.Active)
        {
            AbilityBase[] abilities =
                type == EAbilityType.Active ?
                GameManager.Player.abilityInstances.Where(ability => ability is ActiveAbilityBase).ToArray() :
                GameManager.Player.abilityInstances.Where(ability => ability is PassiveAbilityBase).ToArray();

            m_entries = new UIAbilityListEntry[abilities.Length];

            for (int i = 0; i < abilities.Length; ++i)
            {
                AbilityBase ability = abilities[i];

                GameObject entryInstance = Instantiate(m_abilityBarEntryPrefab, m_abilityListRoot.transform);
                UIAbilityListEntry entry = entryInstance.GetComponent<UIAbilityListEntry>();

                if (entry)
                {
                    entry.Initialize(ability.baseAbilitySheet, type);
                    m_entries[i] = entry;
                }
            }
        }

        private void EnterEquipMode(UIAbilityListEntry ability)
        {
            m_abilitySelected = ability;

            if (!GameManager.InputSystem.IsPointerActive(EActionMap.UI))
            {
                m_abilityBar.SelectFirstElement();
            }

            m_toEnableWhenEquippingAnAbility.ForEach(go => go.SetActive(true));
            m_listCanvasGroup.interactable = false;
        }

        private void ExitEquipMode(UIAbilityListEntry toSelect = null)
        {
            m_toEnableWhenEquippingAnAbility.ForEach(go => go.SetActive(false));
            m_abilitySelected = null;
            m_listCanvasGroup.interactable = true;

            if (toSelect != null && !GameManager.InputSystem.IsPointerActive(EActionMap.UI))
            {
                toSelect.ForceSelection();
            }
        }

        private void OnAbilityHovered(AbilitySheet sheet)
        {
            m_description.text = sheet.description;

            List<AbilityDescriptionLine> lines = new();
            sheet.GenerateAdditionalDescriptionLines(lines);

            // Add a line break before printing additional lines
            if (lines.Count > 0)
            {
                m_description.text += "\n";
            }

            foreach (var line in lines)
            {
                string header = !string.IsNullOrEmpty(line.header) ? $"<u>{line.header}</u>: " : string.Empty;
                m_description.text += $"\n{header}{line.content}";
            }
        }

        private void OnNullAbilityHovered()
        {
            m_description.text = string.Empty;
        }

        // Select an ability from the list
        private void OnAbilitySelectedFromList(UIAbilityListEntry ability) => EnterEquipMode(ability);

        private void OnAbilityCategorySelected(EAbilityType type) => SelectCategory(type);

        private void SelectCategory(EAbilityType type)
        {
            foreach (var entry in m_categories)
            {
                entry.Value.SetHighlight(false);
            }

            m_categories[type].SetHighlight(true);

            ClearSpellBookList();
            FillSpellBookList(type);
        }

        private void OnAbilityCategoryHovered(EAbilityType type)
        {
            m_description.text =
                type == EAbilityType.Active ?
                m_activeAbilityDescription :
                m_passiveAbilityDescription;
        }

        // Select a slot to place the selected ability
        private void OnAbilityClicked(int abilityIndex)
        {
            if (m_abilitySelected == null)
            {
                GameManager.Player.Unequip(abilityIndex);
            }
            else
            {
                var ability = m_abilitySelected.GetTarget() as ActiveAbilitySheet;
                Debug.Assert(ability != null, "Selected ability is not an active ability");
                GameManager.Player.Equip(ability, abilityIndex);
                ExitEquipMode(m_abilitySelected);
            }
        }
    }
}
