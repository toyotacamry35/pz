using System;
using System.Linq;
using Assets.Src.Cartographer;
using Assets.Src.Lib.Cheats;
using Src.Debugging;
using UnityEngine;
using Utilities;

namespace Assets.Src.Telemetry
{
    public class TelemetryUIBehaviour : MonoBehaviour
    {
        [SerializeField] private UIFps _fpsMeter;
        [SerializeField] private DebugTimelineChart _fpsGraph;
        [SerializeField] private DebugTimelineChart _memoryGraph;
        [SerializeField] private DebugTimelineChart _allocationsGraph;

        private bool _mustBeInitialized;
        private Mode _mode;
        private bool _pause;
        
        private void OnEnable()
        {
            _mustBeInitialized = true;
        }

        private void OnDisable()
        {
            DebugState.Instance.OnIsVisibleFpsChanged -= OnIsVisibleFpsChanged;
            OnIsVisibleFpsChanged(0);
            _mustBeInitialized = false;
        }

        private void OnIsVisibleFpsChanged(int value)
        {
            _mode = (Mode) value;
            _fpsGraph.enabled = _mode >= Mode.GraphFPS;
            _fpsMeter.enabled = _mode >= Mode.SimpleFPS;
            _fpsMeter.SetUseRect2(_fpsGraph.enabled);
            _memoryGraph.enabled = _mode == Mode.FPSWithMemory;
            _allocationsGraph.enabled = _mode == Mode.FPSWithAllocations;
        }
        
        public void LateUpdate()
        {
            if (_mustBeInitialized)
            {
                _mustBeInitialized = false;
                DebugState.Instance.OnIsVisibleFpsChanged -= OnIsVisibleFpsChanged;
                DebugState.Instance.OnIsVisibleFpsChanged += OnIsVisibleFpsChanged;
                OnIsVisibleFpsChanged(DebugState.Instance.IsVisibleFps);
            }

            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (shift)
            {
                var alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                var altOrCtrl = alt || ctrl;
                if (!ClientCheatsState.DebugInfo)
                { // для всех
                    if (Input.GetKeyDown(KeyCode.F) && ctrl)
                        DebugState.Instance.IsVisibleFps = (DebugState.Instance.IsVisibleFps + 1) % ModesCountForAll;
                }
                else
                { // для разработчиков
                    if (Input.GetKeyDown(KeyCode.Equals) && !altOrCtrl)
                        DebugState.Instance.IsVisibleFps = (DebugState.Instance.IsVisibleFps + 1) % ModesCount;

                    if (Input.GetKeyDown(KeyCode.Minus) && !altOrCtrl)
                        DebugState.Instance.IsVisibleFps = (DebugState.Instance.IsVisibleFps + ModesCount - 1) % ModesCount;

                    if (Input.GetKeyDown(KeyCode.F1) && !altOrCtrl)
                        SceneStreamerSystem.SwitchShowBackgroundObjectsCheat();

                    if (Input.GetKeyDown(KeyCode.F2) && !altOrCtrl)
                        SceneStreamerSystem.SwitchShowBackgroundTerrainCheat();

                    if (Input.GetKeyDown(KeyCode.F3) && !altOrCtrl)
                        SceneStreamerSystem.SwitchShowObjectsCheat();

                    if (Input.GetKeyDown(KeyCode.F4) && !altOrCtrl)
                        SceneStreamerSystem.SwitchShowTerrainCheat();

                    if (Input.GetKeyDown(KeyCode.F5) && !altOrCtrl)
                        SceneStreamerSystem.SwitchShowEffectsCheat();

                    if (Input.GetKeyDown(KeyCode.Pause) && !altOrCtrl)
                        _pause = !_pause;
                }
            }

            if (TelemetrySystem.Telemetry != null)
            {
                if (_mode >= Mode.Off || TelemetrySystem.Telemetry.IsUpdateRequired)
                    TelemetrySystem.Telemetry.Update();

                if (_mode >= Mode.Off && !_pause)
                {
                    _fpsMeter.SetFps(TelemetrySystem.Telemetry.AverageFPS);
                    _fpsGraph.Sample(TelemetrySystem.Telemetry.FrameTime, Time.unscaledTime);
                    _memoryGraph.Sample(TelemetrySystem.Telemetry.AllocatedMemoryTotal, Time.unscaledTime);
                    _allocationsGraph.Sample(TelemetrySystem.Telemetry.AllocatedMemoryOnFrame, Time.unscaledTime);
                }
            }
        }

        private enum Mode
        {
            Off, 
            SimpleFPS, 
            GraphFPS, 
            FPSWithMemory,
            FPSWithAllocations,
        }

        private static readonly int ModesCount = (int)Enum.GetValues(typeof(Mode)).Cast<Mode>().Max() + 1;
        private static readonly int ModesCountForAll = (int)Mode.GraphFPS + 1;
    }
}
