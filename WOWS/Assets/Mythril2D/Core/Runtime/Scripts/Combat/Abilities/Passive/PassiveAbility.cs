namespace Gyvr.Mythril2D
{
    public abstract class PassiveAbility<SheetType> : PassiveAbilityBase where SheetType : PassiveAbilitySheet
    {
        public SheetType passiveAbilitySheet => (SheetType)m_sheet;
    }
}
