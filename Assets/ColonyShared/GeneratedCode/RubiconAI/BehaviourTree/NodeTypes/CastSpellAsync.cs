using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using SharedCode.Wizardry;
using NLog;
using SharedCode.Utils;
using System.Threading.Tasks;
using SharedCode.Serializers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class CastSpell : BehaviourNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private Legionary _targetAgent;
        private Vector3? _targetPoint;
        private CastSpellDef _def;
        private SpellId _currentOrder;
        private DeltaDictionaryChangedDelegate<SpellId, ISpellClientFull> _onOrderFinished;
        private enum SpellState
        {
            None,
            Running,
            Failed,
            Succeeded
        }
        private bool _isAwaiting = false;
        private SpellState _spellState;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (CastSpellDef)def;
            _currentOrder = default;
            _spellState = SpellState.None;
            var repo = HostStrategy.CurrentLegionary.Repository;
            var oRef = HostStrategy.CurrentLegionary.Ref;
            _onOrderFinished = OnOrderFinished;
        }
        public override async ValueTask<ScriptResultType> OnStart()
        {
            StatusDescription = null;
            _currentOrder = default;
            _spellState = SpellState.None;
            var targetExpr = (TargetSelector)await _def.At.ExprOptional(HostStrategy);
            if (targetExpr != null)
            {
                _targetAgent = await targetExpr.SelectTarget(HostStrategy.CurrentLegionary);
                _targetPoint = await targetExpr.SelectPoint(HostStrategy.CurrentLegionary);
            }
            _chosenSpell = _def.Spell.Target;
            if (_targetAgent != null && !_targetAgent.IsValid)
                _targetAgent = null;
            if (_chosenSpell == null || _chosenSpell == null)
            {
                if (Status != null) StatusDescription = "No spell";
                return ScriptResultType.Failed;
            }
            if (_chosenSpell.RequireParameterOfType<SpellTargetDef>() && _targetAgent == null)
            {
                if (Status != null) StatusDescription = "No target " + _chosenSpell.____GetDebugShortName();
                return ScriptResultType.Failed;
            }
            if (_chosenSpell.RequireParameterOfType<SpellVector3Def>() && !_targetPoint.HasValue && _targetAgent == null)
            {
                if (Status != null) StatusDescription = "No point" + _chosenSpell.____GetDebugShortName();
                return ScriptResultType.Failed;
            }
            if (!_targetPoint.HasValue && _targetAgent != null)
                _targetPoint = HostStrategy.CurrentLegionary.GetPos(_targetAgent);
            if (Status != null)
                StatusDescription = _chosenSpell.____GetDebugShortName();
            _spellState = SpellState.Running;
            var spellBuilder = new SpellCastBuilder().SetSpell(_chosenSpell);
            if (_chosenSpell.RequireParameterOfType<SpellTargetDef>())
            {
                if (_targetAgent == null)
                {
                    if (Status != null) StatusDescription = "No target " + _chosenSpell.____GetDebugShortName();
                    if (Logger.IsDebugEnabled) Logger.IfError()?.Message($"No target for spell {_chosenSpell.____GetDebugShortName()} ").Write();
                    return ScriptResultType.Failed;
                }
                spellBuilder.SetTarget(_targetAgent.Ref);
            }
            if (_chosenSpell.RequireParameterOfType<SpellTargetPointDef>())
            {
                if (!_targetPoint.HasValue)
                {
                    if (Status != null) StatusDescription = "No point" + _chosenSpell.____GetDebugShortName();
                    if (Logger.IsDebugEnabled) Logger.IfError()?.Message($"No point for spell {_chosenSpell.____GetDebugShortName()}").Write();
                    return ScriptResultType.Failed;
                }
                spellBuilder.SetPosition3(_targetPoint.Value);
            }
            
            if (Status != null) StatusDescription = $"Try {_chosenSpell.____GetDebugShortName()}";
            SpellCast order = spellBuilder.Build();
            var repo = HostStrategy.CurrentLegionary.Repository;
            var oRef = HostStrategy.CurrentLegionary.Ref;
            using (var wrap = await repo.Get<IWizardEntityClientFull>(oRef.Guid))
            {
                var wizard = wrap.Get<IWizardEntityClientFull>(oRef.Guid);
                if (wizard == null)
                    return ScriptResultType.Failed;
                lock (this)
                {
                    _isAwaiting = true;
                    wizard.Spells.OnItemRemoved -= _onOrderFinished;
                    wizard.Spells.OnItemRemoved += _onOrderFinished;
                }
                _currentOrder = await wizard.CastSpell(order);
                if (_currentOrder == default)
                {
                    lock (this)
                    {
                        _isAwaiting = false;
                        wizard.Spells.OnItemRemoved -= _onOrderFinished;
                    }
                }
            }
            if (_currentOrder != default)
            {
                //Logger.IfError()?.Message($"Casted spell {_spellState}").Write();
                if (_spellState == SpellState.Running)
                    return ScriptResultType.Running;
                else
                {
                    if (_spellState == SpellState.Failed)
                        return ScriptResultType.Failed;
                    else if (_spellState == SpellState.Succeeded)
                        return ScriptResultType.Succeeded;
                    return ScriptResultType.Failed;
                }
            }
            else
            {
                //Logger.IfError()?.Message($"Failed to cast spell {_spellState}").Write();
                _spellState = SpellState.None;
                if (Status != null) StatusDescription = "Can't cast";
                return ScriptResultType.Failed;
            }
        }

        public override async ValueTask OnFinish()
        {
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            lock (this)
            {

                if (_spellState == SpellState.Succeeded)
                    return ScriptResultType.Succeeded;
                else if (_spellState == SpellState.Failed)
                {
                    if(_def.TreatFailedSpellAsSuccess)
                        return ScriptResultType.Succeeded;
                    else 
                        return ScriptResultType.Failed;
                
                }
                    if (Status != null) StatusDescription = $"{_chosenSpell.____GetDebugShortName()}";
                return ScriptResultType.Running;

            }
        }

        private Task OnOrderFinished(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientFull> args)
        {
            lock (this)
            {
                //Logger.IfError()?.Message($"OnOrderFinished {_currentOrder} {args.Value.Id} {args.Value.CastData.Def.____GetDebugShortName()}").Write();
                if (_currentOrder != args.Value.Id)
                    return Task.CompletedTask;
                var finishReason = args.Value.FinishReason;
                if (finishReason == SpellFinishReason.FailOnStart)
                {
                    HostStrategy.ShouldTickWithDelay(1f);
                    _spellState = SpellState.Failed;
                }
                else if (finishReason == SpellFinishReason.FailOnDemand || finishReason == SpellFinishReason.FailOnEnd)
                {
                    HostStrategy.ShouldTickImmediately();
                    _spellState = SpellState.Failed;
                }
                _spellState = SpellState.Succeeded;
                _currentOrder = default;
                var repo = HostStrategy.CurrentLegionary.Repository;
                var oRef = HostStrategy.CurrentLegionary.Ref;
                _isAwaiting = false;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrap = await repo.Get<IWizardEntityClientFull>(oRef.Guid))
                    {
                        var wizard = wrap.Get<IWizardEntityClientFull>(oRef.Guid);
                        if (wizard == null)
                            return;
                        lock (this)
                        {
                            if (!_isAwaiting)
                                wizard.Spells.OnItemRemoved -= _onOrderFinished;
                        }
                    }
                }, repo);
            }
            return Task.CompletedTask;

        }

        private SpellDef _chosenSpell;

        public override async ValueTask OnTerminate()
        {
            if (_currentOrder != null)
            {
                var ord = _currentOrder;
                var repo = HostStrategy.CurrentLegionary.Repository;
                var oRef = HostStrategy.CurrentLegionary.Ref;
                using (var wrap = await repo.Get<IWizardEntityClientFull>(oRef.Guid))
                {
                    var wizard = wrap.Get<IWizardEntityClientFull>(oRef.Guid);
                    if (wizard == null)
                        return;
                    wizard.Spells.OnItemRemoved -= _onOrderFinished;
                    await wizard.StopCastSpell(_currentOrder, SpellFinishReason.FailOnDemand);
                }
                _currentOrder = default;
                _spellState = SpellState.Failed;

            }
        }

    }
}