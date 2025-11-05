using System.Threading.Tasks;

namespace Gyvr.Mythril2D
{
    public interface IItemEffect
    {
        public Task<bool> TryUse(Item item, CharacterBase target, EItemLocation location);
    }
}
