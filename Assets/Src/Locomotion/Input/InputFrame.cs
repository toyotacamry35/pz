using System;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public struct InputFrame
    {
        private readonly ArraySegment<float> _axes;
        private readonly uint _triggers;

        public readonly float DeltaTime;
 
        public float this[InputAxis i] => _axes.Get(i.Index);

        public Vector2 this[InputAxes i] => new Vector2(_axes.Get(i.First.Index), _axes.Get(i.Second.Index));

        public bool this[InputTrigger t] => (_triggers & t.Mask) != 0;

        public InputFrame(ArraySegment<float> axes, uint triggers, float dt)
        {
            _axes = axes;
            _triggers = triggers;
            DeltaTime = dt;
        }           
    }
}