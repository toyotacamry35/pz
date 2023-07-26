namespace Assets.Src.Effects.FX
{
    public class FXElementParams : IGameObjectPoolParams
    {
        public bool CanLoop { get; set; } = false;
        public bool CheckRotationParam { get; set; } = false;
        public AkGameObj AkGameObject { get; set; }
    }
}