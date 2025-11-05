using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public static class CheckpointUtil
    {
        public static string GetActualMapName(string map)
        {
            if (string.IsNullOrEmpty(map))
            {
                return GameManager.MapSystem.GetCurrentMapName();
            }
            
            return map;
        }
    }
}
