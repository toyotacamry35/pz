using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode;

namespace Src.Locomotion
{
    public class DebugProxy : ILocomotionDebugAgent, ILocomotionDebugInfoProvider
    {
        private Value[] _frame = new Value[DebugTags.Count];
        private Value[] _backFrame = new Value[DebugTags.Count];
        private bool _isDemanded;
        
        public Value this[DebugTag id] => _frame[id.Idx()];

        public IEnumerable<DebugTag> Tags => _frame.Where((x,i) => !x.IsNone).Select((x,i) => DebugTags.FromIdx(i));

        public bool Contains(DebugTag id) => !_frame[id.Idx()].IsNone;

        public bool IsActive => _isDemanded;

        public bool IsDemanded { set => _isDemanded = value; }

        public void Add(DebugTag id, Value value)
        {
            if (!IsActive)
                return;
            _backFrame[id.Idx()] = value;
        }

        public void BeginOfFrame() {}

        public void EndOfFrame()
        {
            var tmp = _backFrame;
            _backFrame = _frame;
            _frame = tmp;
            var empty = default(Value);
            for (int i = 0; i < _backFrame.Length; i++)
                _backFrame[i] = empty;
        }
    }
}
