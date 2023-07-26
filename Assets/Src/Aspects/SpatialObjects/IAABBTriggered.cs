namespace Assets.Src.Aspects.SpatialObjects
{
    public interface IAABBTriggered
    {
        void OnAABBEnter();
        void OnAABBExit();
    }
}