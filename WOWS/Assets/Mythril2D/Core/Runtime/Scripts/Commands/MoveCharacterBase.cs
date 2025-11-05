using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public abstract class MoveCharacterBase : ICommand
    {
        protected abstract CharacterBase targetCharacter { get; }

        [SerializeField] private Vector2 m_movement;

        public async Task Execute()
        {
            Debug.Assert(targetCharacter != null, "Missing character reference!");
            Vector3 initialPosition = targetCharacter.transform.position;
            Vector3 targetPosition = initialPosition + (Vector3)m_movement;
            await targetCharacter.MoveTo(targetPosition).Task;
        }
    }
}
