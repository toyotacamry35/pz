namespace Src.Locomotion
{
    public struct InputAxes
    {
        public readonly InputAxis First, Second;

        public InputAxes(InputAxis first, InputAxis second)
        {
            First = first;
            Second = second;
        }
        
        public static bool operator ==(InputAxes a, InputAxes b) => a.First == b.First && a.Second == b.Second;
        
        public static bool operator !=(InputAxes a, InputAxes b) => !(a == b);
    }
}