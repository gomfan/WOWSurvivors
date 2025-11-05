using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Abilities + nameof(SummoningAbilitySheet))]
    public class SummoningAbilitySheet : ActiveAbilitySheet
    {
        [Header("Summoning Ability Settings")]
        [SerializeField] private GameObject m_toSummon = null;
        [SerializeField] private bool m_followSummoner = true;
        [SerializeField] private bool m_matchSummonerAlignment = true;
        [SerializeField] private bool m_matchSummonerInvincibilityOnHit = true;
        [SerializeField] private bool m_increaseLevelToMatchSummoner = true;
        [SerializeField] private int m_maxSummonCount = 1;
        [SerializeField] private AbilitySheet[] m_bonusAbilities = null;

        public GameObject toSummon => m_toSummon;
        public bool followSummoner => m_followSummoner;
        public bool matchSummonerAlignment => m_matchSummonerAlignment;
        public bool matchSummonerInvincibilityOnHit => m_matchSummonerInvincibilityOnHit;
        public bool increaseLevelToMatchSummoner => m_increaseLevelToMatchSummoner;
        public int maxSummonCount => m_maxSummonCount;
        public AbilitySheet[] bonusAbilities => m_bonusAbilities;
    }
}
