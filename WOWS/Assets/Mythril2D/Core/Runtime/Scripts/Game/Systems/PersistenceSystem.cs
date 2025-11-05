using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PersistenceDataBlock : DataBlock
    {
        [SerializeReference] public PersistableDataBlock[] objects;
    }

    public class PersistenceSystem : AGameSystem, IDataBlockHandler<PersistenceDataBlock>
    {
        internal struct InstanstiationResult
        {
            public Persistable persistable;
            public string identifier;
        }

        private Dictionary<string, PersistableDataBlock> m_preInstanced = new();
        private Dictionary<string, PersistableDataBlock> m_runtimeInstanced = new();

        private Dictionary<string, Persistable> m_persistables = new();

        public override void OnSystemStart()
        {
            GameManager.NotificationSystem.persistableDestroyed.AddListener(OnPersistableDestroyed);
        }

        public override void OnSystemStop()
        {
            GameManager.NotificationSystem.persistableDestroyed.RemoveListener(OnPersistableDestroyed);
        }

        public override void OnMapLoaded()
        {
            LoadPreInstancedDataBlocks();
            LoadRuntimeInstancedDataBlocks();
        }

        public override void OnMapUnloading()
        {
            TakeSnapshotOfPersistableDataBlocks(true);
        }

        public Persistable GetPersistable(string identifier)
        {
            identifier = GetActualIdentifier(identifier);

            if (!string.IsNullOrEmpty(identifier) && m_persistables.ContainsKey(identifier))
            {
                return m_persistables[identifier];
            }

            return null;
        }

        // Instantiate a persistable object from a prefab reference, and automatically save it to respawn it on load
        public Persistable InstantiateRuntime(PrefabReference instance, Vector3 position, Quaternion rotation, Transform parent = null, string identifier = null)
        {
            InstanstiationResult result = InstantiateInternal(instance.prefab, position, rotation, parent, identifier);
            result.persistable.MakeRuntimeInstanced(instance, result.identifier);
            return result.persistable;
        }

        // Instantiate a persistable object from a prefab, but leave the saving responsibility to the caller
        public Persistable InstantiateCustom(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, string identifier = null)
        {
            InstanstiationResult result = InstantiateInternal(prefab, position, rotation, parent, identifier);
            result.persistable.MakeCustomInstanced(result.identifier);
            return result.persistable;
        }

        internal InstanstiationResult InstantiateInternal(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, string identifier = null)
        {
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString();
            }

            GameObject go = Instantiate(prefab, position, rotation, parent);
            Persistable persistable = go.GetComponent<Persistable>();
            Debug.Assert(persistable != null, "The prefab doesn't contain a persistent object!");
            m_persistables[identifier] = persistable;

            return new()
            {
                persistable = persistable,
                identifier = identifier
            };
        }

        public void RegisterCustomInstancedPersistable(Persistable persistable, string identifier = null)
        {
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString();
            }

            m_persistables[identifier] = persistable;
            persistable.MakeCustomInstanced(identifier);
        }

        public void LoadDataBlock(PersistenceDataBlock block)
        {
            if (block.objects != null)
            {
                foreach (var objectBlock in block.objects)
                {
                    Debug.Assert(objectBlock.info is not ManualPersistentDataHandler, "Manual data blocks shouldn't be saved in the persistence system");

                    switch (objectBlock.info)
                    {
                        case PreInstancedPersistentDataHandler preInstancedPersistentDataHandler:
                            m_preInstanced[preInstancedPersistentDataHandler.identifier] = objectBlock;
                            break;
                        case RuntimeInstancedPersistentDataHandler runtimeInstancedPersistentDataHandler:
                            m_runtimeInstanced[runtimeInstancedPersistentDataHandler.identifier] = objectBlock;
                            break;
                    }
                }
            }
        }

        public PersistenceDataBlock CreateDataBlock()
        {
            TakeSnapshotOfPersistableDataBlocks();

            return new PersistenceDataBlock
            {
                objects = m_preInstanced.Values.Union(m_runtimeInstanced.Values).ToArray()
            };
        }

        private string GetActualIdentifier(string identifier)
        {
            if (GameManager.Config.persistentIdentifierMappings.ContainsKey(identifier))
            {
                return GameManager.Config.persistentIdentifierMappings[identifier];
            }

            return identifier;
        }

        private PersistableDataBlock GetDataBlock(string identifier)
        {
            m_preInstanced.TryGetValue(
                GetActualIdentifier(identifier),
                out PersistableDataBlock dataBlock
            );

            return dataBlock;
        }

        private void StorePreInstancedDataBlock(PersistableDataBlock block)
        {
            Debug.Assert(block.info is PreInstancedPersistentDataHandler, "StorePreInstancedDataBlock() expected a pre-instanced data handler");
            m_preInstanced[((PreInstancedPersistentDataHandler)block.info).identifier] = block;
        }

        private void RemoveRuntimeInstancedDataBlock(string identifier)
        {
            m_runtimeInstanced.Remove(identifier);
        }

        private void EvaluateRuntimeInstancedDataBlock(PersistableDataBlock block)
        {
            Debug.Assert(block.info is RuntimeInstancedPersistentDataHandler, "StoreRuntimeInstancedDataBlock() expected a runtime-instanced data handler");

            if (block.state == EPersistableObjectState.Destroyed)
            {
                m_runtimeInstanced.Remove(((RuntimeInstancedPersistentDataHandler)block.info).identifier);
            }
            else
            {
                m_runtimeInstanced[((RuntimeInstancedPersistentDataHandler)block.info).identifier] = block;
            }
        }

        private void OnPersistableDestroyed(Persistable persistable)
        {
            if (persistable.IsAutomaticallyPersisted())
            {
                if (persistable.IsPreInstanced())
                {
                    StorePreInstancedDataBlock(persistable.CreateDataBlock());
                }
                else if (persistable.IsRuntimeInstanced())
                {
                    string identifier = ((RuntimeInstancedPersistentDataHandler)persistable.persistenceInfo).identifier;
                    m_persistables.Remove(identifier);
                    RemoveRuntimeInstancedDataBlock(identifier);
                }
            }
        }

        private void LoadPreInstancedDataBlocks()
        {
            foreach (var persistable in FindObjectsByType<Persistable>(FindObjectsSortMode.InstanceID))
            {
                if (persistable.IsPreInstanced())
                {
                    string identifier = ((PreInstancedPersistentDataHandler)persistable.persistenceInfo).identifier;

                    m_persistables[identifier] = persistable;

                    PersistableDataBlock block = GameManager.PersistenceSystem.GetDataBlock(identifier);
                    if (block != null)
                    {
                        persistable.LoadDataBlock(block);
                    }
                }
            }
        }

        private void LoadRuntimeInstancedDataBlocks()
        {
            string currentMap = GameManager.MapSystem.GetCurrentMapName();

            var keysToRemove = new List<string>();

            foreach (var kvp in m_runtimeInstanced)
            {
                PersistableDataBlock block = kvp.Value;
                Debug.Assert(block.info is RuntimeInstancedPersistentDataHandler, "Expected a runtime instanced data handler!");
                var handler = (RuntimeInstancedPersistentDataHandler)block.info;
                if (handler.map == currentMap)
                {
                    Debug.Assert(handler.prefab != null, "RuntimeInstanced persistable has no prefab reference");
                    Persistable persistable = InstantiateRuntime(handler.prefab, Vector3.zero, Quaternion.identity, null, handler.identifier);
                    persistable.LoadDataBlock(block);
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                m_runtimeInstanced.Remove(key);
            }
        }

        // Register all pre-instanced & runtime-instanced objects manually (in prevision of a save)
        private void TakeSnapshotOfPersistableDataBlocks(bool disablePersistence = false)
        {
            foreach (var persistable in FindObjectsByType<Persistable>(FindObjectsSortMode.InstanceID))
            {
                if (persistable.IsAutomaticallyPersisted())
                {
                    if (persistable.IsPreInstanced())
                    {
                        StorePreInstancedDataBlock(persistable.CreateDataBlock());
                        if (disablePersistence)
                        {
                            persistable.DisablePersistence();
                        }
                    }
                    else if (persistable.IsRuntimeInstanced())
                    {
                        EvaluateRuntimeInstancedDataBlock(persistable.CreateDataBlock());
                        if (disablePersistence)
                        {
                            persistable.DisablePersistence();
                        }
                    }
                }
            }
        }
    }
}
