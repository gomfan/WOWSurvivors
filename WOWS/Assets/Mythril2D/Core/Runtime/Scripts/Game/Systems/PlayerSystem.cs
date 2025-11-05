using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PlayerDataBlock : DataBlock
    {
        public DatabaseEntryReference<PrefabReference> instance;
        public HeroDataBlock heroData;
    }

    public class PlayerSystem : AGameSystem, IDataBlockHandler<PlayerDataBlock>
    {
        [Header("Settings")]
        [SerializeField] private GameObject m_dummyPlayerPrefab = null;

        public Hero PlayerInstance => m_playerInstance;
        public PrefabReference Instance => m_instance;

        private Hero m_playerInstance = null;
        private PrefabReference m_instance = null;

        public override void OnSystemStart()
        {
            m_playerInstance = InstantiatePlayer(m_dummyPlayerPrefab);

            GameManager.NotificationSystem.heroKilled.AddListener(OnHeroKilled);
        }

        public override void OnSystemStop()
        {
            GameManager.NotificationSystem.heroKilled.RemoveListener(OnHeroKilled);
        }

        private void OnHeroKilled(Hero hero)
        {
            if (hero == m_playerInstance)
            {
                GameManager.DialogueSystem.Main.Interrupt();
                GameManager.NotificationSystem.closeAllMenusRequested.Invoke();
                Debug.Assert(GameManager.Config.toExecuteOnPlayerDeath != null, "No action specified to execute on player death! Specify an action in the GameConfig.");
                GameManager.Config.toExecuteOnPlayerDeath.Execute();
                GameManager.DialogueSystem.Main.Interrupt();
            }
        }

        private Hero InstantiatePlayer(GameObject prefab, PlayerDataBlock block = null)
        {
            Persistable instance = GameManager.PersistenceSystem.InstantiateCustom(prefab, block?.heroData.position ?? Vector3.zero, Quaternion.identity, transform);
            Hero hero = instance as Hero;
            GameManager.PersistenceSystem.RegisterCustomInstancedPersistable(hero, Constants.UniquePlayerIdentifier);
            Debug.Assert(hero != null, "The player instance specified doesn't have a Hero component");
            return hero;
        }

        private Hero CreatePlayer(PrefabReference instance, PlayerDataBlock block = null, bool invokePlayerSpawnedEvent = true)
        {
            Hero hero = InstantiatePlayer(instance.prefab, block);
            m_instance = instance;

            if (block != null)
            {
                hero.LoadDataBlock(block.heroData);
            }

            if (invokePlayerSpawnedEvent)
            {
                GameManager.NotificationSystem.playerSpawned.Invoke(hero);
            }

            return hero;
        }

        public void LoadDataBlock(PlayerDataBlock block)
        {
            if (m_playerInstance)
            {
                Destroy(m_playerInstance.gameObject);
            }

            PrefabReference instance = GameManager.Database.LoadFromReference(block.instance);
            Debug.Assert(instance != null, "The player instance reference is invalid");
            m_playerInstance = CreatePlayer(instance, block);
        }

        public PlayerDataBlock CreateDataBlock()
        {
            return new PlayerDataBlock
            {
                instance = GameManager.Database.CreateReference(m_instance),
                heroData = m_playerInstance.CreateDataBlockManual().As<HeroDataBlock>()
            };
        }
    }
}
