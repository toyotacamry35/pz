using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils.DebugCollector;
using Src.Locomotion;
using UnityEngine;

namespace Src.Input
{
    public class InputListener
    {
        private static readonly NLog.Logger LoggerTriggers = LogManager.GetLogger(nameof(InputListener) + ".Triggers");
        private static readonly NLog.Logger LoggerValues = LogManager.GetLogger(nameof(InputListener) + ".Values");
        
        private readonly IInputSource[] _sources;
        private readonly float _threshold;
        private readonly float _holdTime;
        private State _state;
        private float _pressedAt;
        private int _pressedCountdown;
        private float _prevValue;
        private string _keyName;

        public InputListener([NotNull] IEnumerable<IInputSource> sources, float threshold = 0.01f, float holdTime = 0.5f)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            _sources = sources.ToArray();
            _threshold = threshold;
            _holdTime = holdTime;
        }

        /// <summary>
        /// Значение ввода 
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Кнопка нажата  
        /// </summary>
        public bool Pressed => _state != State.Released;

        /// <summary>
        /// Кнопка была нажата в течении holdTime или дольше
        /// </summary>
        public bool Holded => _state == State.Holded;

        /// <summary>
        /// Кнопка была нажата на последнем апдейте
        /// </summary>
        public bool PressEvent { get; private set; }

        /// <summary>
        /// Кнопка была отпущена на последнем апдейте
        /// </summary>
        public bool ReleaseEvent { get; private set; }

        /// <summary>
        /// На последнем апдейте кнопка была отпущена после короткого нажатия (меньше LongClickTime) 
        /// </summary>
        public bool ClickEvent { get; private set; }

        /// <summary>
        /// На последнем апдейте время нажатия кнопки сравнялось с holdTime
        /// </summary>
        public bool HoldEvent { get; private set; }

        public IEnumerable<InputSourceDef> Sources => _sources.Select(x => x.Def);
        
        public void Update()
        {
            PressEvent = false;
            ReleaseEvent = false;
            ClickEvent = false;
            HoldEvent = false;

            float value = 0;
            foreach (var source in _sources)
                value += source.Value;
            Value = value;

            if (LoggerValues.IsDebugEnabled && !Value.ApproximatelyEqual(_prevValue, 0.01f))
            {
                LoggerValues.IfDebug()?.Message($"{this} | {_prevValue} -> {Value} Δ:{Value - _prevValue}").Write();
                _prevValue = Value;
            }
            
            var oldState = _state;
            if (Value > _threshold)
            {
                if (oldState == State.Released)
                {
                    _state = State.Pressed;
                    _pressedAt = Time.unscaledTime;
                    _pressedCountdown = 3;
                    PressEvent = true;
                    if (LoggerTriggers.IsDebugEnabled) LoggerTriggers.IfDebug()?.Message($"{this} | Press Event").Write();
                    Collect.IfActive?.Event($"InputListener.Press.{this}");
                }
                else
                if (oldState == State.Pressed )
                {
                    if (--_pressedCountdown <= 0 && Time.unscaledTime > _pressedAt + _holdTime)
                    {
                        _state = State.Holded;
                        HoldEvent = true;
                        if (LoggerTriggers.IsDebugEnabled) LoggerTriggers.IfDebug()?.Message($"{this} | Hold Event | Duration:{Time.unscaledTime - _pressedAt} HoldTime:{_holdTime} Counter:{_pressedCountdown}").Write();
                        Collect.IfActive?.Event($"InputListener.Hold.{this}");
                    }
                    else
                    {
                        if (LoggerTriggers.IsTraceEnabled) LoggerTriggers.IfTrace()?.Message($"{this} | Still Pressed | Duration:{Time.unscaledTime - _pressedAt} HoldTime:{_holdTime} Counter:{_pressedCountdown}").Write();
                    }
                }
            }
            else
            {
                _state = State.Released;
                if (oldState != State.Released)
                {
                    ReleaseEvent = true;
                    if (LoggerTriggers.IsDebugEnabled) LoggerTriggers.IfDebug()?.Message($"{this} | Release Event").Write();
                    Collect.IfActive?.Event($"InputListener.Release.{this}");
                }

                if (oldState == State.Pressed)
                {
                    ClickEvent = true;
                    if (LoggerTriggers.IsDebugEnabled) LoggerTriggers.IfDebug()?.Message($"{this} | Click Event").Write();
                    Collect.IfActive?.Event($"InputListener.Click.{this}");
                }
            }
        }

        public void Reset()
        {
            PressEvent = false;
            ReleaseEvent = false;
            ClickEvent = false;
            HoldEvent = false;
            Value = 0;
            _pressedAt = 0;
            _prevValue = 0;
            _state = State.Released;
        }
        
        public override string ToString()
        {
            if(_keyName == null)
                _keyName = string.Join(",", (object[]) _sources);
            return _keyName;
        }        

        enum State { Released, Pressed, Holded }
    }
}
