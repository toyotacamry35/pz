using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUpdate;

namespace Uins.Cursor
{
    public static class CursorControl
    {
        public class Token : IDisposable
        {
            private readonly object _owner;

            public Token(object owner)
            {
                _owner = owner;
            }

            #region IDisposable Support
            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (disposedValue)
                    return;

                Remove(this);

                disposedValue = true;
            }

            ~Token()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion

            public override string ToString()
            {
                return $"{GetType().Name}({_owner})";
            }
        }

        private static readonly HashSet<object> _freeRequests = new HashSet<object>();
        private static readonly HashSet<object> _lockRequests = new HashSet<object>();


        //=== Props ===============================================================

        private static bool IsCursorUnlockedByRequests => _freeRequests.Count >= _lockRequests.Count;

        private static bool CursorForceUnlockSwitch =>
            (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt)) ||
            (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt)) ||
            (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftAlt));

        private static bool IsCursorTempUnlocked => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        private static bool CursorLocked
        {
            set
            {
                UnityEngine.Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                UnityEngine.Cursor.visible = !value;
            }
        }

        private static bool _isCursorForceUnlocked;

        //=== Public ==============================================================

        public static Token AddCursorFreeRequest(object owner)
        {
            var token = new Token(owner);
            _freeRequests.Add(token);
            return token;
        }

        public static Token AddCursorLockRequest(object owner)
        {
            var token = new Token(owner);
            _lockRequests.Add(token);
            return token;
        }

        private static void Remove(Token token)
        {
            if (_freeRequests.Remove(token))
                return;

            if (_lockRequests.Remove(token))
                return;

            throw new InvalidOperationException($"Unknown token: {token}");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                UnityUpdateDelegate.OnUpdate += Update;
        }

        private static void Update()
        {
            if (CursorForceUnlockSwitch)
                _isCursorForceUnlocked = !_isCursorForceUnlocked;

            bool isCursorLocked = !IsCursorUnlockedByRequests && !_isCursorForceUnlocked && !IsCursorTempUnlocked &&  SharedCode.Aspects.Item.Templates.Constants.WorldConstants.LockCursor;

            CursorLocked = isCursorLocked;
        }
    }
}