using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public struct SimpleCheckpoint : ICheckpoint
    {
        [MapSelector] public string map;
        public Vector3 position;

        string ICheckpoint.map => map;
        Vector3 ICheckpoint.position => position;
        public bool IsValid() => true;
        public void UpdateMapName() => map = CheckpointUtil.GetActualMapName(map);
    }
}
