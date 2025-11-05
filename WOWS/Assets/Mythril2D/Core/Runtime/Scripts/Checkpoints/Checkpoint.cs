namespace Gyvr.Mythril2D
{
    public class Checkpoint : Persistable
    {
        public ICheckpoint GetData()
        {
            return new PersistableCheckpoint()
            {
                map = GameManager.MapSystem.GetCurrentMapName(),
                instance = this
            };
        }
    }
}
