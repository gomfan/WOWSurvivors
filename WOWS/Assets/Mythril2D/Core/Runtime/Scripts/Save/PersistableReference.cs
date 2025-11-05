using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public struct PersistableReference<T> where T : Persistable
    {
        public T instance =>
            !string.IsNullOrEmpty(m_identifier) ?
            GameManager.PersistenceSystem.GetPersistable(m_identifier) as T :
            null;

        public string identifier => m_identifier;

        [SerializeField] private string m_identifier;

        public PersistableReference(T instance) => this = instance;

        public static implicit operator T(PersistableReference<T> reference) => reference.instance;

        public static implicit operator PersistableReference<T>(T instance) => new()
        {
            m_identifier =
                instance?.persistenceInfo is IIdentifiablePersistentDataHandler handler ?
                handler.GetIdentifier() :
                null
        };
    }
}