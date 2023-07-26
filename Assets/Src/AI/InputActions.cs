using Assets.Src.Aspects.Doings;
using Assets.Src.Shared.Impl;
using System;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class InputActions : BehaviourNode
    {
        private BotBrain _botBrain;
        private InputActionsDef _selfDef;
        private DateTime _startTime;
        private object causer;

        private ValueTask<ScriptResultType> runnungValueTask = new ValueTask<ScriptResultType>(ScriptResultType.Running);

        protected override ValueTask OnCreate(BehaviourNodeDef def)
        {
            _selfDef = def as InputActionsDef;
            return new ValueTask();
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            _startTime = DateTime.Now;
            causer = new object();
            var @ref = HostStrategy.CurrentLegionary.Ref;
            bool failed = false;
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                var targetGo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(@ref);
                _botBrain = targetGo.GetComponentInChildren<BotBrain>();

                if (_botBrain == null)
                    failed = true;
            });
            if (failed)
                return ScriptResultType.Failed;
            foreach (var defInputAction in _selfDef.InputActions)
                _botBrain.BotActions.PushTrigger(causer, defInputAction);

            return ScriptResultType.Running;
        }

        protected override ValueTask<ScriptResultType> OnTick()
        {
            if (_selfDef.DurationSeconds <= 0 || (_selfDef.DurationSeconds > 0 && (DateTime.Now - _startTime).TotalSeconds >= _selfDef.DurationSeconds))
            {
                foreach (var defInputAction in _selfDef.InputActions)
                    _botBrain.BotActions.PopTrigger(causer, defInputAction);

                return new ValueTask<ScriptResultType>(ScriptResultType.Succeeded);
            }

            return runnungValueTask;
        }
    }
}
