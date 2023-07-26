using System;
using ColonyShared.SharedCode.Input;
using JetBrains.Annotations;
using NLog;
using UnityEngine;

namespace Src.Input
{
    public class InputSourceAxisRdpWorkaround: IInputSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(Input));

        private float _lastPos;
        private readonly InputAxisRdpWorkaroundDef _def;

        public InputSourceAxisRdpWorkaround([NotNull] InputAxisRdpWorkaroundDef def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            _def = def;
        }

        public InputSourceDef Def => _def;

        public float Value
        {
            get
            {
                float pos = 0;
                switch (_def.Axis)
                {
                    case InputAxisRdpWorkaroundDef.AxisType.X:
                        pos = UnityEngine.Input.mousePosition.x;                        
                        break;
                    case InputAxisRdpWorkaroundDef.AxisType.Y:
                        pos = UnityEngine.Input.mousePosition.y;                              
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var v = pos - _lastPos;
                _lastPos = pos;
                
                return v * _def.Sensitivity;
            }
        }

        public override string ToString()
        {
            return $"Axis:{_def.Axis}";
        }
    }
}