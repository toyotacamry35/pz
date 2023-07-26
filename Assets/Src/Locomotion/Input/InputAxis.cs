namespace Src.Locomotion
{
    public struct InputAxis
    {
        public readonly int Index;

        public InputAxis(int index)
        {
            Index = index;
        }
        
        public static readonly InputAxis None = new InputAxis(-1);
        
        public static bool operator ==(InputAxis a, InputAxis b) => a.Index == b.Index;
        
        public static bool operator !=(InputAxis a, InputAxis b) => !(a == b);
    }
}