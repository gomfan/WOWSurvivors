using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public struct PersistableCheckpoint : ICheckpoint
    {
        [MapSelector] public string map;
        public PersistableReference<Checkpoint> instance;

        public Vector3 position => instance.instance.transform.position;
        string ICheckpoint.map => map;
        public bool IsValid() => !string.IsNullOrEmpty(instance.identifier);
        public void UpdateMapName() => map = CheckpointUtil.GetActualMapName(map);
    }
}
