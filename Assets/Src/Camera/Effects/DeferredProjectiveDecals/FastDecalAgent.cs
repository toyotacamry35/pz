namespace Assets.Src.Camera.Effects.DeferredProjectiveDecals
{
    public class FastDecalAgent
    {
        public readonly FastDecal Decal;
        public FastDecalAgent Next;
        public bool InList;

        public FastDecalAgent(FastDecal decal)
        {
            Decal = decal;
        }
    }
}