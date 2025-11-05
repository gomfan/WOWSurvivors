using UnityEngine;

namespace Gyvr.Mythril2D
{
    public static class CollisionDispatcher
    {
        public static void RegisterCollision(Movable source, GameObject target)
        {
            target.SendMessage("OnMovableCollision", source, SendMessageOptions.DontRequireReceiver);
        }
    }
}
