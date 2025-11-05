using System.Threading.Tasks;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Utils + nameof(CommandHandler))]
    public class CommandHandler : DatabaseEntry
    {
        [SerializeReference, SubclassSelector]
        private ICommand m_command = null;

        public Task Execute() => m_command.Execute();
    }
}
