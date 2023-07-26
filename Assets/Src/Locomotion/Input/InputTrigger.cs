using System;

namespace Src.Locomotion
{
    public struct InputTrigger
    {
        public readonly int Index;
        public readonly uint Mask;

        public InputTrigger(int index)
        {
            if(index >= 32)
                throw new ArgumentException($"Trigger index is too large: {index}");
            Index = index;
            Mask = 1U << index;
        }

        public static InputTrigger None = new InputTrigger(-1);         
        
        public static bool operator ==(InputTrigger a, InputTrigger b) => a.Index == b.Index;
        
        public static bool operator !=(InputTrigger a, InputTrigger b) => !(a == b);
    }
}