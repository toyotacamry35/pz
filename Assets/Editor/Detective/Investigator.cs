using Assets.Src.AI;
using Assets.Src.SpawnSystem;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Detective.Editor
{
    [InitializeOnLoad]
    public static class Investigator
    {
        private static bool _debug = false;
        private static Investigation _investigation;
        private static InvestigatorUpdator _updator;

        static Investigator()
        {
            _curInvestigatedFrame = 0;
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (Application.isPlaying)
            {
                _debug = true;
                if (_updator == null)
                {
                    var go = new GameObject("InvestigatorUpdator");
                    _updator = go.AddComponent<InvestigatorUpdator>();
                    _updator.OnUpdate += BeginUpdate;
                }
            }
        }

        private static GameObject _currentObject;
        private static int _investigatedObjects;
        private static int _curInvestigatedFrame;
        private static Guid _curId;
        public static void BeginUpdate()
        {
            /*if(_curInvestigatedFrame != 0)
            {
                if(_investigation != null)
                {
                    Office.Visit().FinishInvestigation(_investigation);
                    _investigation = null;
                }
            }*/
            if (!_debug)
                return;
            var currentActive = Selection.activeGameObject;
            if (currentActive == null)
                return;
            var o = currentActive.GetComponent<InvestigatorProxy>();
            if (o == null)
                return;
            if (_currentObject == currentActive)
                return;
            else
            {
                if (_currentObject != null)
                {
                    foreach (var inv in _currentObject.GetComponents<InvestigatorProxy>())
                    {
                        var host = inv.Host;
                        if(host != null)
                        {
                            host.OnFrameStart -= OnFrameBegin;
                            host.OnFrameEnd -= OnFrameEnd;
                        }
                    }
                }
                _currentObject = currentActive;
                
            }
            if (_currentObject == null)
                return;
            var invs = _currentObject.GetComponents<InvestigatorProxy>();
            _curId= _currentObject.GetComponent<EntityGameObject>().OuterRef.Guid;
            _investigatedObjects = invs.Length;
            foreach (var inv in invs)
            {
                inv.Host.OnFrameStart += OnFrameBegin;
                inv.Host.OnFrameEnd += OnFrameEnd;
            }
        }

        private static void OnFrameEnd()
        {
            if (_investigation == null)
                return;
            if (_curInvestigatedFrame == _investigatedObjects)
            {
                Office.Visit().FinishInvestigation(_investigation);
                _investigation = null;
            }
            _curInvestigatedFrame = 0;
        }

        private static void OnFrameBegin()
        {
            if (_curInvestigatedFrame == 0)
            {
                _investigation = Office.Visit().BeginInvestigation(_currentObject, _curId);
            }
            _curInvestigatedFrame++;
        }
    }
}

