namespace Gyvr.Mythril2D
{
    public abstract class ActiveAbility<SheetType> : ActiveAbilityBase where SheetType : ActiveAbilitySheet
    {
        public SheetType activeAbilitySheet => (SheetType)m_sheet;
    }
}
