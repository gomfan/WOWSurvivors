using System;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class IControllerDataBlock : DataBlock
    {

    }

    public interface IController : IDataBlockHandler<IControllerDataBlock>
    {
        public void Initialize(Movable movable);
        public void Terminate();
        public void Start();
        public void Stop();
        public void FixedUpdate();
        public void Update();
        public void DrawGizmos();
    }
}
