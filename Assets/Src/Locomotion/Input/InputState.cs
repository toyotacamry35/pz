using System;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public class InputState<TInputs> : IInputState<TInputs> where TInputs : Inputs, new()
    {
        private static readonly TInputs Inputs = new TInputs();
        private readonly float[] _axes = new float[Inputs.AxisCount];
        private readonly bool[] _axesModified = new bool[Inputs.AxisCount];
        private uint _triggers;
        private uint _triggersModified;

        public bool this[InputTrigger t]
        {
            set
            {
                if (value) _triggers |= t.Mask;
                else _triggers &= ~t.Mask;
                _triggersModified |= t.Mask;
            }
            get { return (_triggers & t.Mask) != 0; }
        }

        public float this[InputAxis i]
        {
            set
            {
                _axes[i.Index] = value;
                _axesModified[i.Index] = true;
            }
            get { return _axes[i.Index]; }
        }

        public Vector2 this[InputAxes i]
        {
            set
            {
                _axes[i.First.Index] = value.x;
                _axesModified[i.First.Index] = true;
                _axes[i.Second.Index] = value.y;
                _axesModified[i.Second.Index] = true;
            }
            get { return new Vector2(_axes[i.First.Index],_axes[i.Second.Index]) ; }
        }

        public float this[string inputName]
        {
            set
            {
                var nfo = Inputs.GetInputInfo(inputName);
                switch (nfo.Type)
                {
                    case InputType.Axis:
                        this[nfo.Axis] = value;
                        break;
                    case InputType.Trigger:
                        this[nfo.Trigger] = value > 0.0001;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(inputName)} == '{inputName}'");
                }
            }
        }
 
        public void Clear()
        {
            Array.Clear(_axes, 0, _axes.Length);
            Array.Clear(_axesModified, 0, _axesModified.Length);
            _triggers = 0;
            _triggersModified = 0;
        }

        public bool DBG_IsClear()
        {
            if (!Consts.IsDebug)
                throw new Exception("Should not be called at !DEBUG, 'cos could hit performance");

            return _axes.All(x => x == default)
                && _axesModified.All(x => x == default)
                && _triggers == 0
                && _triggersModified == 0;
        }

        public void ApplyFrom(InputState<TInputs> state)
        {
            if (state._axes.Length != _axes.Length) throw new InvalidOperationException($"Axes count mismatch: {state._axes.Length} vs {_axes.Length}");
            for (int i = 0; i < _axes.Length; ++i)
                if (state._axesModified[i])
                {
                    _axes[i] = state._axes[i];
                    _axesModified[i] = true;
                }
            _triggers = state._triggers & state._triggersModified | _triggers & ~state._triggersModified;
            _triggersModified |= state._triggersModified;
        }
        
        float[] IInputState<TInputs>.Axes => _axes;

        uint IInputState<TInputs>.Triggers => _triggers;

        public override string ToString()
        {
            int i = 0;
            return string.Join(",\n", _axes.Select(x => $"[{i++}]: {x}"));
        }
    }
}
