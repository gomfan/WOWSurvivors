using System.Threading.Tasks;

namespace Gyvr.Mythril2D
{
    public interface IInteraction
    {
        public Task<bool> TryExecute(CharacterBase source, IInteractionTarget target);
    }
}
