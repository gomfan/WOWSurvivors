using UnityEngine;

namespace Gyvr.Mythril2D
{
    public interface ICheckpoint
    {
        public string map { get; }
        public Vector3 position { get; }

        public bool IsValid();
        public void UpdateMapName(); // If the map is set to "Current" (string.Empty or null), this method will update the map name to the current map.
    }
}
