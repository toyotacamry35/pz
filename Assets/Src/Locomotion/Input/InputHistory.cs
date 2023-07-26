using System;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public class InputHistory<TInputs> : ILocomotionInputProvider where TInputs : Inputs, new()
    {
        private static readonly TInputs Inputs = new TInputs();
        private static readonly InputFrame EmptyFrame = new InputFrame(new ArraySegment<float>(new float[Inputs.AxisCount]), 0, 0);
        private readonly RingBuffer<InputFrame> _frames;
        private readonly FramedRingBuffer<float> _axes;

        public InputHistory(int capacity)
        {
            _frames = new RingBuffer<InputFrame>(capacity);
            _axes = new FramedRingBuffer<float>(Inputs.AxisCount, capacity);
            InitFrames();
        }

        public InputHistory<TInputs> Push(IInputState<TInputs> state, float dt)
        {
            _axes.PushFront(state.Axes);
            _frames.PushFront(new InputFrame(_axes.Front(), state.Triggers, dt));
            return this;
        }

        public void Clean()
        {
            _axes.Clear();
            _frames.Clear();
            InitFrames();
        }

        private void InitFrames() => _frames.PushFront(new InputFrame(_axes.PushFront(), 0, 0));

        float ILocomotionInputProvider.this[InputAxis it] => _frames.Front()[it];

        Vector2 ILocomotionInputProvider.this[InputAxes it] => _frames.Front()[it];

        bool ILocomotionInputProvider.this[InputTrigger it] => _frames.Front()[it];

        public int HistoryCount => _frames.Count;

        public InputFrame History(int idx) => _frames[idx];
    }
}