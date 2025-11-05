using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Gyvr.Mythril2D
{
    [RequireComponent(typeof(SpriteLibrary))]
    public class EquipmentSpriteLibraryUpdater : MonoBehaviour
    {
        private SpriteLibrary m_spriteLibrary = null;
        private SpriteLibraryAsset m_defaultSpriteLibraryAsset = null;

        private void Awake()
        {
            m_spriteLibrary = GetComponent<SpriteLibrary>();
            m_defaultSpriteLibraryAsset = m_spriteLibrary.spriteLibraryAsset;
        }

        public void UpdateVisual(CharacterBase character, EEquipmentType equipmentType)
        {
            if (character is Hero)
            {
                Hero hero = (Hero)character;

                if (hero.equipments.TryGetValue(equipmentType, out Equipment equipedWeapon) && equipedWeapon.visualOverride)
                {
                    m_spriteLibrary.spriteLibraryAsset = equipedWeapon.visualOverride;
                }
            }
            else
            {
                m_spriteLibrary.spriteLibraryAsset = m_defaultSpriteLibraryAsset;
            }
        }

        public void ResetVisual()
        {
            m_spriteLibrary.spriteLibraryAsset = m_defaultSpriteLibraryAsset;
        }
    }
}
