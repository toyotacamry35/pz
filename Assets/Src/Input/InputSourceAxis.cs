using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Input;
using JetBrains.Annotations;
using NLog;
using UnityEngine;

namespace Src.Input
{
    public class InputSourceAxis : IInputSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(Input));

        private readonly string _originalAxisName;
        private readonly string _axisName;
        private readonly float _min = float.MinValue;
        private readonly float _max = float.MaxValue;
        private readonly float _scale = 1;

        public InputSourceAxis([NotNull] InputAxisDef def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));

            Def = def;
            
            _originalAxisName = def.Axis;
            
            if (_originalAxisName.EndsWith("+"))
            {
                _min = 0;
                _axisName = _originalAxisName.Substring(0, _originalAxisName.Length - 1);
            }
            else if (_originalAxisName.EndsWith("-"))
            {
                _max = 0;
                _scale = -1;
                _axisName = _originalAxisName.Substring(0, _originalAxisName.Length - 1);
            }
            else
            {
                _axisName = _originalAxisName;
            }
        }

        public InputSourceDef Def { get; }

        public float Value
        {
            get
            {
                var v = UnityEngine.Input.GetAxis(_axisName);
                return Mathf.Clamp(v, _min, _max) * _scale;
            }
        }

        public override string ToString()
        {
            return $"Axis:{_originalAxisName}";
        }
    }
}
