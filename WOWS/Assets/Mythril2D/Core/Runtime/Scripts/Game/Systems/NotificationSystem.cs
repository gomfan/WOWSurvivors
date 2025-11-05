using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public class NotificationSystem : AGameSystem
    {
        [Header("Gameplay Events")]
        public UnityEvent<MonsterSheet> monsterKilled = new();
        public UnityEvent<Hero> heroKilled = new();
        public UnityEvent<CharacterBase, DamageInputDescriptor, EEffectVisualFlags> damageApplied = new();
        public UnityEvent<CharacterBase, ITemporalEffect> temporalEffectApplied = new();
        public UnityEvent<CharacterBase, int, EEffectVisualFlags> healthRecovered = new();
        public UnityEvent<CharacterBase, int, EEffectVisualFlags> manaConsumed = new();
        public UnityEvent<CharacterBase, int, EEffectVisualFlags> manaRecovered = new();
        public UnityEvent<int> experienceGained = new();
        public UnityEvent<int> levelUp = new();
        public UnityEvent<AIController, CharacterBase> targetDetected = new();
        public UnityEvent<int> moneyAdded = new();
        public UnityEvent<int> moneyRemoved = new();
        public UnityEvent<Item, int, EItemTransferType> itemAdded = new();
        public UnityEvent<Item, int, EItemTransferType> itemRemoved = new();
        public UnityEvent<Equipment> itemEquipped = new();
        public UnityEvent<Equipment> itemUnequipped = new();
        public UnityEvent<AbilitySheet> abilityAdded = new();
        public UnityEvent<AbilitySheet> abilityRemoved = new();
        public UnityEvent<Quest> questProgressionUpdated = new();
        public UnityEvent<Quest> questStarted = new();
        public UnityEvent<Quest> questUnlocked = new();
        public UnityEvent<Quest, bool> questAvailabilityChanged = new();
        public UnityEvent<Quest> questFullfilled = new();
        public UnityEvent<Quest> questCompleted = new();
        public UnityEvent<string, bool> gameFlagChanged = new();
        public UnityEvent mapTransitionStarted = new();
        public UnityEvent mapTransitionCompleted = new();
        public UnityEvent mapLoading = new();
        public UnityEvent mapLoaded = new();
        public UnityEvent mapUnloading = new();
        public UnityEvent mapUnloaded = new();
        public UnityEvent<Hero> playerSpawned = new();
        public UnityEvent<Persistable> persistableDestroyed = new();
        public UnityEvent<ITriggerableAbility, EAbilityFireCheckResult> playerFireFailed = new();
        public UnityEvent<MapLoadingDelegationParams> mapTransitionDelegationRequested = new();
        public UnityEvent saveFileLoaded = new();

        [Header("User Interface")]
        public UnityEvent<Shop, TaskCompletionSource<bool>> shopRequested = new();
        public UnityEvent<CraftingStation, TaskCompletionSource<bool>> craftRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> gameMenuRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> statsRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> inventoryRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> journalRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> spellBookRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> settingsRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> saveMenuRequested = new();
        public UnityEvent<TaskCompletionSource<bool>> deathScreenRequested = new();
        public UnityEvent closeAllMenusRequested = new();
        public UnityEvent<IUIMenu> menuShowed = new();
        public UnityEvent<IUIMenu> menuHid = new();
        public UnityEvent<Item> itemDetailsOpened = new();
        public UnityEvent itemDetailsClosed = new();

        [Header("Audio")]
        public UnityEvent<AudioClipResolver> audioPlaybackRequested = new();
    }
}
