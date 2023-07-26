using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using UnityEngine;

namespace Src.Input
{
    public class InputSourceKey : IInputSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(Input));
        
        private readonly KeyCode _keyCode;
        
        public InputSourceKey([NotNull] InputKeyDef def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            
            Def = def;

            if(!Enum.TryParse(def.Key, true, out _keyCode))
                Logger.IfError()?.Message($"Invalid key code '{def.Key}' at {def.____GetDebugAddress()}").Write();
        }

        public InputSourceDef Def { get; }

        public float Value => UnityEngine.Input.GetKey(_keyCode) ? 1 : 0; 
        
        public override string ToString()
        {
            return $"Key:{_keyCode}";
        }
    }
}
