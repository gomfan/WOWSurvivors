using System.Threading.Tasks;

namespace Gyvr.Mythril2D
{
    public interface ICommand
    {
        public Task Execute();
    }
}
