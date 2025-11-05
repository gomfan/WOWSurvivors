using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ChestDataBlock : EntityDataBlock
    {
        public bool opened;
    }

    public class Chest : Entity
    {
        [Header("References")]
        [SerializeField] private Animator m_chestAnimator = null;
        [SerializeField] private Animator m_contentAnimator = null;
        [SerializeField] private SpriteRenderer m_contentSpriteRenderer = null;

        [Header("Chest Settings")]
        [SerializeField] private ChestLoot m_loot;
        [SerializeField] private string m_openedAnimationParameter = "opened";
        [SerializeField] private string m_contentRevealAnimationParameter = "reveal";
        [SerializeField] private float m_contentRevealIconCycleDuration = 1.0f;
        [SerializeField] private DialogueSequence m_noItemDialogue = null;
        [SerializeField] private DialogueSequence m_hasItemDialogue = null;

        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_openingSound;

        private bool m_hasOpeningAnimation = false;
        private bool m_hasRevealAnimation = false;
        private bool m_opened = false;

        protected void Awake()
        {
            Debug.Assert(m_chestAnimator, ErrorMessages.InspectorMissingComponentReference<Animator>());
            Debug.Assert(m_contentAnimator, ErrorMessages.InspectorMissingComponentReference<Animator>());
            Debug.Assert(m_contentSpriteRenderer, ErrorMessages.InspectorMissingComponentReference<SpriteRenderer>());

            if (m_chestAnimator)
            {
                m_hasOpeningAnimation = AnimationUtils.HasParameter(m_chestAnimator, m_openedAnimationParameter);
            }

            if (m_contentAnimator)
            {
                m_hasRevealAnimation = AnimationUtils.HasParameter(m_contentAnimator, m_contentRevealAnimationParameter);
            }
        }

        protected virtual void Start()
        {
            if (m_opened)
            {
                TryPlayOpeningAnimation(true);
            }
        }

        public bool TryPlayOpeningAnimation(bool open)
        {
            if (m_chestAnimator && m_hasOpeningAnimation)
            {
                m_chestAnimator.SetBool(m_openedAnimationParameter, open);
                return true;
            }

            return false;
        }

        public bool TryPlayContentRevealAnimation()
        {
            if (m_contentSpriteRenderer && m_contentAnimator && m_hasRevealAnimation)
            {
                Sprite[] sprites = m_loot.GetLootSprites();

                if (sprites.Length > 0)
                {
                    StartCoroutine(UpdateContentSprite(sprites, m_contentRevealIconCycleDuration));
                    m_contentAnimator.SetTrigger(m_contentRevealAnimationParameter);
                    return true;
                }

                return false;
            }

            return false;
        }

        private IEnumerator UpdateContentSprite(Sprite[] sprites, float duration)
        {
            if (sprites.Length == 0) yield break;

            float interval = duration / sprites.Length;

            for (int index = 0; index < sprites.Length; ++index)
            {
                m_contentSpriteRenderer.sprite = sprites[index];
                yield return new WaitForSeconds(interval);
            }
        }

        public async Task<bool> TryOpen()
        {
            if (!m_opened)
            {
                TryPlayOpeningAnimation(true);
                TryPlayContentRevealAnimation();

                if (!m_loot.IsEmpty())
                {
                    GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_openingSound);

                    if (m_loot.entries != null)
                    {
                        foreach (var entry in m_loot.entries)
                        {
                            GameManager.DialogueSystem.Main.AddToQueue(
                                m_hasItemDialogue.ToDialogueTree(
                                    string.Empty, $"{entry.item.displayName} x{entry.quantity}"
                                )
                            );

                            GameManager.InventorySystem.AddToBag(entry.item, entry.quantity, EItemTransferType.Chest);
                        }

                        if (m_loot.money != 0)
                        {
                            GameManager.DialogueSystem.Main.AddToQueue(
                                m_hasItemDialogue.ToDialogueTree(
                                    string.Empty, $"{m_loot.money} <currency.fullName>"
                                )
                            );

                            GameManager.InventorySystem.AddMoney(m_loot.money);
                        }
                    }
                }
                else
                {
                    GameManager.DialogueSystem.Main.AddToQueue(
                        m_noItemDialogue.ToDialogueTree(string.Empty)
                    );
                }

                await GameManager.DialogueSystem.Main.PlayQueue();

                m_opened = true;

                return true;
            }

            return false;
        }

        protected override Type GetDataBlockType() => typeof(ChestDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<ChestDataBlock>().opened = m_opened;
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            m_opened = block.As<ChestDataBlock>().opened;
        }
    }
}
