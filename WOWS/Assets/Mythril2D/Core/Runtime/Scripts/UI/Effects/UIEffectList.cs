using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UIEffectList : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject m_buffEffectEntryPrefab = null;
        [SerializeField] private GameObject m_debuffEffectEntryPrefab = null;
        [SerializeField] private GameObject m_listContentRoot = null;
        [SerializeField] private UIEffectDescription m_effectDescription = null;

        public void Show()
        {
            HideDescriptionPanel();

            foreach (Transform child in m_listContentRoot.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var temporalEffect in GameManager.Player.temporalEffects)
            {
                var effectEntry = Instantiate(
                    temporalEffect.GetEffectType() == EEffectType.Buff ?
                    m_buffEffectEntryPrefab :
                    m_debuffEffectEntryPrefab,
                    m_listContentRoot.transform);
                effectEntry.GetComponent<UIEffectListEntry>().SetEffect(temporalEffect);
            }
        }

        public void Hide()
        {
            HideDescriptionPanel();
        }

        private void ShowDescriptionPanel(ITemporalEffect effect, float positionY) => m_effectDescription?.Show(effect, positionY);
        private void HideDescriptionPanel() => m_effectDescription?.Hide();

        // Message received from UIEffectListEntry
        private void OnEffectHovered(EffectHoveredEvent eventData) => ShowDescriptionPanel(eventData.effect, eventData.listElementY);
        // Message received from UIEffectListEntry
        private void OnEffectNotHovered() => HideDescriptionPanel();
    }
}
