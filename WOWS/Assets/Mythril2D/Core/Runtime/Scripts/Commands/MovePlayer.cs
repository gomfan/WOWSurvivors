using System;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class MovePlayer : MoveCharacterBase
    {
        protected override CharacterBase targetCharacter => GameManager.Player;
    }
}
