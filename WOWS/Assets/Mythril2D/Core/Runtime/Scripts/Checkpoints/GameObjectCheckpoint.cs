using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public struct GameObjectCheckpoint : ICheckpoint
    {
        [MapSelector] public string map;
        public string gameObjectName;

        public Vector3 position => GameObject.Find(gameObjectName)?.transform.position ?? Vector3.zero;
        string ICheckpoint.map => map;
        public bool IsValid() => !string.IsNullOrEmpty(gameObjectName);
        public void UpdateMapName() => map = CheckpointUtil.GetActualMapName(map);
    }
}
