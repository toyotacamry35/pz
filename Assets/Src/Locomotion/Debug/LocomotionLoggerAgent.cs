using ColonyShared.SharedCode.Aspects.Locomotion;
using System.Text;
using ColonyShared.SharedCode;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public class LocomotionLoggerAgent : ILocomotionDebugAgent
    {
        private static readonly LocomotionLogger Logger = LocomotionLogger.Default;
        private static readonly StringBuilder Buffer = new StringBuilder();
        
        private string _prevState, _newState;
        private LocomotionFlags _prevFlags, _newFlags;
        private bool _prevCheatMode;
        private bool _prevDirectControl;

        public bool IsActive => Logger.IsDebugEnabled;
        
        public void Add(DebugTag id, Value entry)
        {
            if (!Logger.IsDebugEnabled)
                return;
            
            switch (id)
            {
                case StateMachineStateName:
                    _newState = entry.String;
                    break;
                case MovementFlags:
                    _newFlags = (LocomotionFlags)entry.Int;
                    break;
                case CheatMode:
                    var cheatMode = entry.Bool;
                    if (_prevCheatMode != cheatMode)
                    {
                        Logger.IfDebug()?.Message("Cheat Mode: {0}", cheatMode).Write();
                        _prevCheatMode = cheatMode;
                    }
                    break;
                case IsDirectControl:
                    var directControl = entry.Bool;
                    if (_prevDirectControl != directControl)
                    {
                        Logger.IfDebug()?.Message("Direct Control: {0}", directControl).Write();
                        _prevDirectControl = directControl;
                    }
                    break;
            }
        }
        
        public void BeginOfFrame() {}
        
        public void EndOfFrame()
        {
            if (!Logger.IsDebugEnabled)
                return;
            Buffer.Clear();
            bool log = false;
            if (_prevState != _newState)
            {
                Buffer.AppendFormat("{0} -> {1} ", _prevState, _newState);
                _prevState = _newState;
                log = true;
            }
            if (_prevFlags != _newFlags)
            {
                Buffer.AppendFormat("({0} -> {1}) ", _prevFlags, _newFlags);
                _prevFlags = _newFlags;
                log = true;
            }
            if(log)
                Logger.IfDebug()?.Message(Buffer.ToString()).Write();
        }
    }
}
