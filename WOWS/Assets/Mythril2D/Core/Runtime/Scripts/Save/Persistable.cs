using System;
using UnityEngine;
using UnityEngine.Events;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public enum EPersistableObjectState
    {
        Active,
        Inactive,
        Destroyed
    }

    public interface APersistenceInfo
    {
        public bool IsValid();
    }

    public interface IIdentifiablePersistentDataHandler
    {
        public string GetIdentifier();
    }

    [Serializable]
    public class PreInstancedPersistentDataHandler : APersistenceInfo, IIdentifiablePersistentDataHandler
    {
        public string identifier;

        public string GetIdentifier() => identifier;

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(identifier);
        }
    }

    [Serializable]
    public class RuntimeInstancedPersistentDataHandler : APersistenceInfo, IIdentifiablePersistentDataHandler
    {
        public PrefabReference prefab;
        public string map;
        public string identifier;

        public string GetIdentifier() => identifier;

        public bool IsValid()
        {
            return
                prefab != null &&
                !string.IsNullOrEmpty(map) &&
                !string.IsNullOrEmpty(identifier);
        }
    }

    [Serializable]
    public class CustomInstancedPersistentDataHandler : APersistenceInfo, IIdentifiablePersistentDataHandler
    {
        public string identifier;

        public string GetIdentifier() => identifier;

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(identifier);
        }
    }

    [Serializable]
    public class ManualPersistentDataHandler : APersistenceInfo
    {
        public bool IsValid()
        {
            return true;
        }
    }

    [Serializable]
    public class PersistableDataBlock : DataBlock
    {
        [SerializeReference, HideInInspector] public APersistenceInfo info = null;
        public EPersistableObjectState state;
    }

    public class Persistable : MonoBehaviour, IDataBlockHandler<PersistableDataBlock>
    {
        [SerializeField] private bool m_autoPersistWhenPreInstanced = false;
        [SerializeField] private bool m_disablePermanentDestroy = false;
        [SerializeReference, SubclassSelector] private ICommand m_executeOnDeath = null;
        [SerializeReference, HideInInspector] private APersistenceInfo m_persistenceInfo = null;

        public APersistenceInfo persistenceInfo
        {
            get => m_persistenceInfo;
#if UNITY_EDITOR
            set => m_persistenceInfo = value;
#endif
        }

        public UnityEvent destroyedEvent => m_destroyedEvent;

        private bool m_destroyed = false;
        private UnityEvent m_destroyedEvent = new();
        private bool m_forceNoPersistence = false;

        public void DisablePersistence()
        {
            m_forceNoPersistence = true;
        }

        protected bool IsMarkedAsDestroyed()
        {
            return m_destroyed;
        }

        protected void MarkAsDestroyed()
        {
            m_destroyed = true;
        }

        protected void MarkAsNotDestroyed()
        {
            m_destroyed = false;
        }

        public virtual void Destroy()
        {
            MarkAsDestroyed();
            GameManager.NotificationSystem.persistableDestroyed.Invoke(this);
            m_destroyedEvent.Invoke();
            m_executeOnDeath?.Execute();
            Destroy(gameObject);
        }

        public bool IsPersistent()
        {
            return
                !m_forceNoPersistence &&
                m_persistenceInfo != null &&
                m_persistenceInfo.IsValid();
        }

        public bool IsAutomaticallyPersisted()
        {
            return
                IsPreInstanced() && m_autoPersistWhenPreInstanced ||
                IsRuntimeInstanced();
        }

        public bool IsPreInstanced()
        {
            return
                IsPersistent() &&
                m_persistenceInfo is PreInstancedPersistentDataHandler;
        }

        public bool IsRuntimeInstanced()
        {
            return
                IsPersistent() &&
                m_persistenceInfo is RuntimeInstancedPersistentDataHandler;
        }

        public bool IsCustomInstanced()
        {
            return
                IsPersistent() &&
                m_persistenceInfo is CustomInstancedPersistentDataHandler;
        }

        public bool IsIdentifiable()
        {
            return
                IsPersistent() &&
                m_persistenceInfo is IIdentifiablePersistentDataHandler;
        }

        public void MakeRuntimeInstanced(PrefabReference instance, string identifier)
        {
            m_persistenceInfo = new RuntimeInstancedPersistentDataHandler
            {
                prefab = instance,
                map = GameManager.MapSystem.GetCurrentMapName(),
                identifier = identifier
            };
        }

        public void MakeCustomInstanced(string identifier)
        {
            m_persistenceInfo = new CustomInstancedPersistentDataHandler
            {
                identifier = identifier
            };
        }

        public PersistableDataBlock CreateDataBlock()
        {
            return CreateDataBlockBase(m_persistenceInfo);
        }

        public PersistableDataBlock CreateDataBlockManual()
        {
            return CreateDataBlockBase(new ManualPersistentDataHandler());
        }

        public PersistableDataBlock CreateDataBlockBase(APersistenceInfo info)
        {
            Debug.Assert(info != null && info.IsValid(), "Cannot save data block with missing persistence info");

            PersistableDataBlock block = (PersistableDataBlock)Activator.CreateInstance(GetDataBlockType());

            block.info = info;
            block.state =
                m_destroyed ?
                EPersistableObjectState.Destroyed :
                    gameObject.activeInHierarchy ?
                    EPersistableObjectState.Active :
                    EPersistableObjectState.Inactive;

            OnSave(block);
            return block;
        }

        public void LoadDataBlock(PersistableDataBlock block)
        {
            switch (block.state)
            {
                case EPersistableObjectState.Active:
                    gameObject.SetActive(true);
                    break;
                case EPersistableObjectState.Inactive:
                    gameObject.SetActive(false);
                    break;
                case EPersistableObjectState.Destroyed:
                    if (!m_disablePermanentDestroy) Destroy(gameObject);
                    return; // <-- return to avoid calling OnLoad
            }

            OnLoad(block);
        }

        protected virtual Type GetDataBlockType() => typeof(PersistableDataBlock);
        protected virtual void OnSave(PersistableDataBlock block) { }
        protected virtual void OnLoad(PersistableDataBlock block) { }
    }
}
