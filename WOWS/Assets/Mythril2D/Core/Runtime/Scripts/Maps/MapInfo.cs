using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public class MapInfo : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private ICheckpoint m_playtestCheckpoint = null;

        public ICheckpoint playtestCheckpoint => m_playtestCheckpoint;
    }
}
