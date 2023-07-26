using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using Uins.Tooltips;
using UnityEngine;
using ColonyDI;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using Src.Input;
using UnityUpdate;

namespace Uins
{
    public class WindowsManager : IDependencyNode, IIsDisposed
    {
        public static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        private Dictionary<WindowId, IGuiWindow> _registeredWindows = new Dictionary<WindowId, IGuiWindow>();
        private Dictionary<WindowStackId, StackInfo> _windowStacks = new Dictionary<WindowStackId, StackInfo>();
        private int _lastWindowsChangeFrameCount;
        private bool _isInGame;
        internal bool IsInGame => _isInGame;
        private Queue<DelayedActionInfo> _delayedActions = new Queue<DelayedActionInfo>();
        private DisposableComposite _d = new DisposableComposite();

        private bool _hasOpenedOverlayWindows;


        //=== Props ===============================================================

        public bool IsDisposed { get; private set; }

        public ResourceUsageNotifier HudCenterPartUsageNotifier = new ResourceUsageNotifier();
        public ResourceUsageNotifier HudLeftPartUsageNotifier = new ResourceUsageNotifier();
        public ResourceUsageNotifier HudRightPartUsageNotifier = new ResourceUsageNotifier();

        public bool IsJustChangedWindowsStates => _lastWindowsChangeFrameCount == Time.frameCount;

        public IDependencyNode Parent { get; set; }

        public IEnumerable<IDependencyNode> Children => Enumerable.Empty<IDependencyNode>();

        /// <summary>
        /// Открыто ли хоть одно окно в _windowStacks за исключением IsUnclosable
        /// </summary>
        public IStream<bool> HasOpenedWindowsButUnclosableStream { get; private set; }

        /// <summary>
        /// Открыто ли хоть одно окно в оверлейных _windowStacks
        /// </summary>
        public IStream<bool> HasOpenedOverlayWindowsStream { get; private set; }


        //=== Ctor ================================================================

        public WindowsManager(WindowStackId[] usedStackIds)
        {
            _d.Add(HudRightPartUsageNotifier);
            _d.Add(HudLeftPartUsageNotifier);
            _d.Add(HudCenterPartUsageNotifier);
            RegisterStackIds(usedStackIds);
            CreateStreams();
            UnityUpdateDelegate.OnUpdate += MainUpdate;
        }


        //=== Public ==============================================================

        public void Dispose()
        {
            IsDisposed = true;
            UnityUpdateDelegate.OnUpdate -= MainUpdate;
            _d.Clear();
            _registeredWindows = null;
            _windowStacks = null;
        }

        public bool RegisterWindow(IGuiWindow guiWindow)
        {
            if (guiWindow.AssertIfNull(nameof(guiWindow)) || !IsUniqueWindow(guiWindow))
                return false;

            _registeredWindows.Add(guiWindow.Id, guiWindow);

            if (guiWindow.IsUnclosable)
                Open(guiWindow);

            return true;
        }

        public IGuiWindow GetWindow(WindowId windowId)
        {
            if (IsDisposed)
                return null;

            IGuiWindow guiWindow;
            if (windowId.AssertIfNull(nameof(windowId)))
                return null;

            if (!_registeredWindows.TryGetValue(windowId, out guiWindow))
                Logger.IfError()?.Message($"{nameof(GetWindow)}() guiWindow with Id={windowId.name} don't registered").Write();

            return guiWindow;
        }

        public void Open(WindowId windowId) => Open(GetWindow(windowId));

        public void Open(IGuiWindow guiWindow, WindowStackId stackIdToOpenWindow = null, object arg = null)
        {
            if (IsDisposed ||
                guiWindow.AssertIfNull(nameof(guiWindow)) ||
                !IsRegisteredWindow(guiWindow) ||
                (stackIdToOpenWindow != null && !IsRegisteredStackId(stackIdToOpenWindow)))
                return;
            
            BaseTooltip.Hide();
            _delayedActions.Enqueue(new DelayedActionInfo()
            {
                WindowAction = OpenWork,
                GuiWindow = guiWindow,
                WindowStackId = stackIdToOpenWindow ?? guiWindow.Id.PrefferedStackId,
                Arg = arg
            });
        }

        public void Close(WindowId windowId) => Close(GetWindow(windowId));

        public void Close(IGuiWindow guiWindow)
        {
            if (IsDisposed)
                return;

            if (guiWindow.AssertIfNull(nameof(guiWindow)) ||
                !IsRegisteredWindow(guiWindow))
            {
                Logger.IfError()?.Message($"{nameof(Close)}({guiWindow}, stack={guiWindow.CurrentWindowStack}) Unable to close guiWindow cause registering/null problem").Write();
                return;
            }

            BaseTooltip.Hide();
            _delayedActions.Enqueue(new DelayedActionInfo()
            {
                WindowAction = CloseWork,
                GuiWindow = guiWindow,
                WindowStackId = guiWindow.CurrentWindowStack
            });
        }

        public bool HasOpenedWindowsInStack(WindowStackId windowStackId, bool butUnclosable = true)
        {
            if (windowStackId.AssertIfNull(nameof(windowStackId)) || !IsRegisteredStackId(windowStackId))
                return false;

            var stack = _windowStacks[windowStackId].Stack;
            if (stack.Count == 0)
                return false;

            return !butUnclosable || !stack.Peek().IsUnclosable;
        }

        public override string ToString()
        {
            return $"{typeof(WindowsManager)}"; //TODO
        }

        public void OnIsInGameChanged(bool isOn)
        {
            _isInGame = isOn;
            if (!isOn)
                CloseAllClosableWindows();
        }

        public void AfterDependenciesInjected()
        {
        }

        public void AfterDependenciesInjectedOnAllProviders()
        {
        }

        public IStream<IGuiWindow> GetCurrentWindowsStream(WindowStackId stackId)
        {
            if (IsDisposed)
                return null;

            if (!_windowStacks.TryGetValue(stackId, out var stackInfo))
            {
                Logger.IfError()?.Message($"Not found StackWindowsStream for {nameof(stackId)}: {stackId}").Write();
                return null;
            }

            return stackInfo.CurrentWindow;
        }


        //=== Private =============================================================

        private void MainUpdate()
        {
            while (_delayedActions.Count > 0)
            {
                var actionInfo = _delayedActions.Dequeue();
                actionInfo.WindowAction(actionInfo.GuiWindow, actionInfo.WindowStackId, actionInfo.Arg);
            }

            if (!_isInGame)
                return;

            OnAlwaysUpdate();

            if (IsJustChangedWindowsStates)
                return;

            foreach (var stackInfo in _windowStacks.Values)
            {
                if (stackInfo.Stack.Count == 0 || (!stackInfo.StackId.CanOverlayOtherStacks && _hasOpenedOverlayWindows))
                    continue;

                stackInfo.Stack.Peek().OpenUpdate();

                foreach (var guiWindow in stackInfo.Stack)
                    guiWindow.NoClosedUpdate();
            }
        }

        private void RegisterStackIds(WindowStackId[] stackIds)
        {
            if (!stackIds.AssertIfNull(nameof(stackIds)))
                foreach (var windowStackId in stackIds)
                    _windowStacks.Add(windowStackId, new StackInfo(windowStackId));
        }

        private void CreateStreams()
        {
            foreach (var stackInfo in _windowStacks.Values)
            {
                //--- HasOpenedWindowsButUnclosableStream
                var hasOpenedWindowInStackStream = stackInfo.CurrentWindow.Func(_d, w => w != null && !w.IsUnclosable);
                if (HasOpenedWindowsButUnclosableStream == null)
                {
                    HasOpenedWindowsButUnclosableStream = hasOpenedWindowInStackStream;
                }
                else
                {
                    HasOpenedWindowsButUnclosableStream =
                        HasOpenedWindowsButUnclosableStream
                            .Zip(_d, hasOpenedWindowInStackStream)
                            .Func(_d, (b1, b2) => b1 || b2);
                }

                //--- HasOpenedOverlayWindowsStream
                if (stackInfo.StackId.CanOverlayOtherStacks)
                {
                    var hasOpenedWindowInOverlayStackStream = stackInfo.CurrentWindow.Func(_d, w => w != null);
                    if (HasOpenedOverlayWindowsStream == null)
                    {
                        HasOpenedOverlayWindowsStream = hasOpenedWindowInOverlayStackStream;
                    }
                    else
                    {
                        HasOpenedOverlayWindowsStream = HasOpenedOverlayWindowsStream
                            .Zip(_d, hasOpenedWindowInOverlayStackStream)
                            .Func(_d, (b1, b2) => b1 || b2);
                    }
                }
            }

            HasOpenedOverlayWindowsStream.Action(_d, b => _hasOpenedOverlayWindows = b);
        }

        private bool IsRegisteredStackId([NotNull] WindowStackId windowStackId)
        {
            var success = _windowStacks.ContainsKey(windowStackId);
            if (!success)
                Logger.IfError()?.Message($"WindowStackId '{windowStackId.name}' isn't registered").Write();
            return success;
        }

        private bool IsUniqueWindow([NotNull] IGuiWindow guiWindow)
        {
            if (guiWindow.Id.AssertIfNull(nameof(guiWindow.Id)))
                return false;

            IGuiWindow existingGuiWindow;
            var success = !_registeredWindows.TryGetValue(guiWindow.Id, out existingGuiWindow);
            if (!success)
                Logger.IfError()?.Message($"{nameof(IsUniqueWindow)}({guiWindow}) guiWindow {existingGuiWindow} with such Id is already registered").Write();
            return success;
        }

        private bool IsRegisteredWindow([NotNull] IGuiWindow guiWindow)
        {
            if (guiWindow.Id.AssertIfNull(nameof(guiWindow.Id)))
                return false;

            var success = _registeredWindows.ContainsKey(guiWindow.Id);
            if (!success)
                Logger.IfError()?.Message($"{nameof(IsRegisteredWindow)}() guiWindow {guiWindow} isn't registered").Write();

            return success;
        }

        private void CloseAllClosableWindows()
        {
            foreach (var stackInfo in _windowStacks.Values)
            {
                var stack = stackInfo.Stack;
                if (stack.Count == 0)
                    continue;

                var lastOpenWindow = stack.Peek();
                while (lastOpenWindow != null && !lastOpenWindow.IsUnclosable)
                {
                    CloseWork(lastOpenWindow, lastOpenWindow.CurrentWindowStack, null);
                    lastOpenWindow = stack.Count == 0 ? null : stack.Peek();
                }
            }
        }

        private void OnAlwaysUpdate()
        {
        }

        private void SetWindowChangeFlag()
        {
            _lastWindowsChangeFrameCount = Time.frameCount;
        }

        private void OpenWork([NotNull] IGuiWindow guiWindow, [NotNull] WindowStackId stackIdToOpenWindow, object arg)
        {
            if (guiWindow.State.Value != GuiWindowState.Closed)
            {
                Logger.IfWarn()?.Message($"{nameof(OpenWork)}({guiWindow}, {stackIdToOpenWindow}) Unable to open guiWindow").Write();
                return;
            }

            SetWindowChangeFlag();

            var stackInfo = _windowStacks[stackIdToOpenWindow];
            var stack = stackInfo.Stack;

            IGuiWindow guiWindowToFade = null;
            if (stack.Count > 0)
            {
                guiWindowToFade = stack.Peek();
                guiWindowToFade.State.Value = GuiWindowState.Faded;
            }

            guiWindow.State.Value = GuiWindowState.Opened;
            guiWindow.CurrentWindowStack = stackIdToOpenWindow;
            stack.Push(guiWindow);

            if (guiWindow.InputBindings != null)
                InputManager.Instance.PushBindings(guiWindow, guiWindow.InputBindings);

            guiWindowToFade?.OnFade();
            guiWindow.OnOpen(arg);

            //Logger.IfDebug()?.Message($"[{Time.frameCount}] {nameof(OpenWork)}({guiWindow})").Write(); //DEBUG
            stackInfo.CurrentWindow.Value = guiWindow;
        }

        private void CloseWork([NotNull] IGuiWindow guiWindow, [NotNull] WindowStackId stackIdToCloseWindow, object arg)
        {
            if (guiWindow.State.Value == GuiWindowState.Closed)
            {
                Logger.IfWarn()?.Message($"{nameof(CloseWork)}({guiWindow}, {stackIdToCloseWindow}) Unable to close already closed guiWindow").Write();
                return;
            }

            SetWindowChangeFlag();

            if (!_windowStacks.TryGetValue(stackIdToCloseWindow, out var stackInfo))
            {
                Logger.IfError()?.Message($"{nameof(CloseWork)}({guiWindow}, {stackIdToCloseWindow}) Unable to close guiWindow, cause stack not found").Write();
                return;
            }

            var stack = stackInfo.Stack;
            IGuiWindow guiWindowToClose = null;
            do
            {
                if (stack.Count == 0)
                {
                    Logger.Error(
                        $"{nameof(CloseWork)}({guiWindow}, {stackIdToCloseWindow}) Unable to close guiWindow, cause stack is empty");
                    return;
                }

                if (stack.Peek().IsUnclosable)
                {
                    Logger.Error($"{nameof(CloseWork)}({guiWindow}, {stackIdToCloseWindow}) Unable to close guiWindow " +
                                 $"cause unclosable window was met: {guiWindowToClose}");
                    return;
                }

                guiWindowToClose = stack.Pop();

                if (guiWindowToClose.InputBindings != null)
                    InputManager.Instance.PopBindings(guiWindowToClose);

                guiWindowToClose.State.Value = GuiWindowState.Closed;
                guiWindowToClose.CurrentWindowStack = null;


                IGuiWindow guiWindowToUnfade = null;
                if (stack.Count > 0)
                {
                    guiWindowToUnfade = stack.Peek();
                    guiWindowToUnfade.State.Value = GuiWindowState.Opened;
                }

                guiWindowToClose.OnClose();
                guiWindowToUnfade?.OnUnfade();

                //Logger.IfDebug()?.Message($"[{Time.frameCount}] {nameof(CloseWork)}({guiWindowToClose}, @{stackIdToCloseWindow})").Write(); //DEBUG
                stackInfo.CurrentWindow.Value = guiWindowToUnfade;
            } while (guiWindowToClose != guiWindow);
        }


        //=== Class =======================================================================================================

        private class DelayedActionInfo
        {
            public Action<IGuiWindow, WindowStackId, object> WindowAction;
            public object Arg;
            public IGuiWindow GuiWindow;
            public WindowStackId WindowStackId;
        }

        private class StackInfo
        {
            public WindowStackId StackId { get; }
            public Stack<IGuiWindow> Stack = new Stack<IGuiWindow>();
            public ReactiveProperty<IGuiWindow> CurrentWindow = new ReactiveProperty<IGuiWindow>();

            private bool _isDisposed;

            public StackInfo(WindowStackId stackId)
            {
                StackId = stackId;
                CurrentWindow.Value = null;
            }
        }
    }
}