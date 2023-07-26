using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.RubiconAI.BehaviourTree;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Src.Detective
{
    // It's debugging
    public class Office
    {
        public Archive Archive => _archive;
        Archive _archive = new Archive(60 * 5);
        private static Office _staticOffice;
        public void SetAsStatic()
        {
            _staticOffice = this;
        }

        public static Office Visit()
        {
            if (_staticOffice == null)
            {
                var office = new Office();
                office.SetAsStatic();
            }
            return _staticOffice;
        }

        private GameObject _lastInvestigatedObject;
        Investigation _currentInvestigation;
        Guid _currentTargetId;
        public Investigation CurrentInvestigation(Guid id)
        {
            if (id == _currentTargetId)
                return _currentInvestigation;
            return null;
        }

        public Investigation BeginInvestigation(GameObject investigatedGameObject, Guid id)
        {
            if (_lastInvestigatedObject != investigatedGameObject)
            {
                _lastInvestigatedObject = investigatedGameObject;
                _archive = new Archive(60 * 5);
            }
            var inv = new Investigation(new ObjectFrameEvent() { Go = investigatedGameObject });
            _currentInvestigation = inv;
            _currentTargetId = id;
            return inv;
        }

        public void FinishInvestigation(Investigation inv)
        {
            _currentInvestigation = null;
            _archive.Put(inv);
        }

    }

    public class ObjectFrameEvent : Event
    {
        public GameObject Go;
    }

}

