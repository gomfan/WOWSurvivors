using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class EntityDataBlock : PersistableDataBlock
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    public class Entity : Persistable, IInteractionTarget
    {
        [Header("Entity Settings")]
        [SerializeField] private UIFloatingIcon m_floatingIcon = null;
        [SerializeReference, SubclassSelector] private IInteraction m_interaction = null;

        public virtual string GetSpeakerName() => string.Empty;

        public void SetFloatingIcon(EFloatingIcon icon, float? duration = null) => m_floatingIcon?.SetIcon(icon, duration);

        public virtual async Task Say(DialogueSequence sequence, UnityAction<DialogueMessageFeed> onDialogueEnded = null, params string[] args)
        {
            string speaker = GetSpeakerName();

            DialogueTree dialogueTree = sequence.ToDialogueTree(speaker, args);

            if (onDialogueEnded != null)
            {
                dialogueTree.dialogueEnded.AddListener(onDialogueEnded);
            }

            await GameManager.DialogueSystem.Main.PlayNow(dialogueTree);
        }

        public virtual void OnInteract(CharacterBase sender)
        {
            m_interaction?.TryExecute(sender, this);
        }

        protected override Type GetDataBlockType() => typeof(EntityDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<EntityDataBlock>().position = transform.position;
            block.As<EntityDataBlock>().rotation = transform.rotation;
            block.As<EntityDataBlock>().scale = transform.localScale;
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            transform.position = block.As<EntityDataBlock>().position;
            transform.rotation = block.As<EntityDataBlock>().rotation;
            transform.localScale = block.As<EntityDataBlock>().scale;
        }
    }
}
