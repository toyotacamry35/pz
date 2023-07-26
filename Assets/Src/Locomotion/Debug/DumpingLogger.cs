using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Extensions;
using SharedCode.Utils.Threads;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.Locomotion.Debug
{
    public interface IDumpingLoggerProvider
    {
        DumpingLogger DLogger { get; }
        //Not good place for it, but let it be here for a while
        int LogBackCounter { get; set; }
    }

    /// <summary>
    /// Collects data at run-time. Can write collected data to file. Can do it by timeout after last logged data.
    /// </summary>
    public class DumpingLogger
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(DumpingLogger));

        public string Name;
        private readonly MonoBehaviour _owner;
        public bool IsActive { get; private set; }
        public bool IsServer;
        private bool _isDumping;
        private Coroutine _autodumpCoroutine;
        private const float AutodumpAfterNoDataTimeout = 10f;
        private float _lastLogLocalTime;
        private static int _counter;

        public const string FolderPath = "/_Logs/DumpingLogs";
        public string FileNameCommon => $"DumpingLogger {Name} _PID-{Process.GetCurrentProcess().Id}";

        private readonly List<LocoVarsHistoryEntry> _history = new List<LocoVarsHistoryEntry>(100);

        public DumpingLogger(string name, MonoBehaviour owner)
        {
            Name = name;
            _owner = owner;
        }

        public DumpingLogger IfActive => IsActive ? this : null;

        public bool Activate() // => _isDumping ? false : IsActive = true;
        {
             Logger.IfError()?.Message("Switch in on in code, if needed. (2 places: here & initiation & 1st activation at CharClView AwakeInt.").Write();;
            return false;
        }

        public void Log(Type t, int index, string tag, LocomotionVariables vars)
        {
            if (!IsActive)
                return;

            _lastLogLocalTime = Time.realtimeSinceStartup;
            if (_autodumpCoroutine == null)
            {
                //DbgLog.Log("DumpingLogger Start AutodumpCoroutine");
                _autodumpCoroutine = _owner.StartCoroutine(AutodumpCoroutine());
            }

            _history.Add(new LocoVarsHistoryEntry(){Type = t, Index = index, Tag = tag, Vars = vars});
        }

        public void DumpToFile() //(bool isServer)
        {
            _isDumping = true;
            IsActive = false;

            Logger.IfDebug()?.Message($"{nameof(DumpingLogger)}. {nameof(DumpToFile)} started. Count:{_history.Count}").Write();

            TaskEx.Run(() =>
            {
                try
                {
                    // 'll not cre. folder, if exist (w/o explicit check)
                    Directory.CreateDirectory(FolderPath);
                    var count = Interlocked.Increment(ref _counter);
                    var fileName = FileNameCommon + (IsServer ? "_S" : "_Cl") +"(" + count + ").log";
                    var logFilePath = Path.Combine(FolderPath, fileName);
                    using (StreamWriter str = File.CreateText(logFilePath))
                    {
                        for (int i = 0;  i < _history.Count;  ++i)
                            str.WriteLine(i + ") " + _history[i].ToString());
                    }

                    Logger.IfInfo()?.Message($"{nameof(DumpingLogger)}. {nameof(DumpToFile)}. File saved to \"{logFilePath}\". (Q.Count: {_history.Count})").Write();
                    //DbgLog.Log($"{nameof(DumpingLogger)}. {nameof(DumpToFile)}. File saved to \"{logFilePath}\". (Q.Count: {_history.Count})");
                }
                finally
                {
                    _history.Clear();
                    _isDumping = false;
                }
            }).WrapErrors();
        }

        private IEnumerator AutodumpCoroutine()
        {
            //DbgLog.Log("DumpingLogger:: Wait: " + AutodumpAfterNoDataTimeout);
            yield return new WaitForSeconds(AutodumpAfterNoDataTimeout + .1f); //+gap

            var wait = AutodumpAfterNoDataTimeout - (Time.realtimeSinceStartup - _lastLogLocalTime);
            while (wait > 0)
            {
                //DbgLog.Log("DumpingLogger:: ReWait: " + wait);
                yield return new WaitForSeconds(wait + .1f); //+gap
                wait = AutodumpAfterNoDataTimeout - (Time.realtimeSinceStartup - _lastLogLocalTime);
            }

            DumpToFile();
            if (_autodumpCoroutine != null)
            {
                _owner.StopCoroutine(_autodumpCoroutine);
                _autodumpCoroutine = null;
            }
        }


        // --- Util types: ---------------

        internal struct LocoVarsHistoryEntry
        {
            internal Type Type;
            internal int Index;
            internal string Tag;
            internal LocomotionVariables Vars;

            public override string ToString() => $"Vars:{Vars} [{Index}] ({Tag}) {Type.Name}";
        }
    }
}
