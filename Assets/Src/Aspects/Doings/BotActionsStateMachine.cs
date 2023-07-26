using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

#if false
namespace Assets.Src.Aspects.Doings
{
    public class BotActionsStateMachine
    {
        private IEnumerator<ResourceRef<BotActionDef>> _stateEnumerator;
        private BotAction _currentStateInstance;
        private BotActionDef _currentState;
        private BotActionsSequenceDef _sequence;
        private bool _looped = false;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Bot");

        public Guid EntityId { get; private set; }
        public Vector3 _camera;
        public Transform _transform;
        public float _time = 0;
        public bool Completed { get; private set; } = false;

        public BotActionsStateMachine(BotActionsSequenceDef botBehaviorJdb, Guid entityID, bool isLooped = true)
        {
            foreach (var action in botBehaviorJdb.Actions)
            {
                Debug.Log(action.ToString());
            }
            _sequence = new BotActionsSequenceDef();
            _sequence.Actions = new List<ResourceRef<BotActionDef>>();
            _sequence.Timings = botBehaviorJdb.Timings;
            foreach (var action in botBehaviorJdb.Actions)
            {
                _sequence.Actions.Add(action);
                _sequence.Actions.Add(_sequence.Timings);
            }
            _stateEnumerator = _sequence.Actions.GetEnumerator();
            EntityId = entityID;
            _looped = isLooped;
        }

        public BotActionsStateMachine(BotActionsSequenceDef actions, Guid entityID, bool looped, BotActionsStateMachine basm)
        {
            foreach (var action in actions.Actions)
            {
                Debug.Log(action.ToString());
            }
            _sequence = new BotActionsSequenceDef();
            _sequence.Actions = new List<ResourceRef<BotActionDef>>();
            _sequence.Timings = basm._sequence.Timings;
            foreach (var action in actions.Actions)
            {
                _sequence.Actions.Add(action);
                _sequence.Actions.Add(_sequence.Timings);
            }
            _stateEnumerator = _sequence.Actions.GetEnumerator();
            EntityId = entityID;
            _looped = looped;
        }

        public BotInputs GetInputs()
        {
            if ((_currentState == null) || (CheckIfNeedToSwicthByDelay(_currentState.delay, _time)) || (CheckIfNeedToSwicthInternal()))
            {
                if (_stateEnumerator.MoveNext() == false)
                {
                    if (!_looped)
                    {
                        Completed = true;
                        return new BotInputs();
                    }

                    _stateEnumerator = _sequence.Actions.GetEnumerator();
                    _stateEnumerator.MoveNext();
                }
                _currentState = _stateEnumerator.Current;
                _time = Time.time;
                _currentStateInstance = (BotAction)Activator.CreateInstance(SharedCode.Utils.DefToType.GetType(_currentState.GetType()), _currentState, this);
                _currentStateInstance.LogAction(this);
            }
            return DoActions();

        }
        public BotInputs DoActions()
        {
            BotInputs inputs = _currentStateInstance.GetInputs();
            return inputs;
        }

        private static bool CheckIfNeedToSwicthByDelay(float delay, float actionStartTime)
        {
            if (delay <= 0)
                return false;
            else if ((actionStartTime + delay) > Time.time)
                return false;
            else
                return true;
        }

        private bool CheckIfNeedToSwicthInternal()
        {
            if (_currentStateInstance == null)
                return true;
            return _currentStateInstance.State != ActionState.Running;
        }
    }
}
#endif