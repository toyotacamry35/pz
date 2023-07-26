using Assets.ColonyShared.SharedCode.Shared;
using Assets.Tools;
using SharedCode.EntitySystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using Assets.Src.Arithmetic;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Serializers;

namespace Assets.Src.Character.Effects
{
    public class FXRuleUpdater
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly (FXRule Rule, FXRuleDef Def)[] _fXs;
        private readonly OuterRef<IEntityObject> _entityRef;
        private readonly int _updatePeriod; //ms
        private CancellationTokenSource _cancellation;
        private bool _disposed;

        public FXRuleUpdater(IEnumerable<FXRuleDef> fxDefs, OuterRef<IEntityObject> entityRef, float updatePeriod, bool onlyMainAnimator = false)
        {
            _fXs = (fxDefs ?? throw new ArgumentNullException(nameof(fxDefs)))
                .Where(fx => !onlyMainAnimator || fx.MainAnimatorProp)
                .Select(fx => ((FXRule) Activator.CreateInstance(SharedCode.Utils.DefToType.GetType(fx.GetType())), fx))
                .ToArray();
            _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
            _updatePeriod = (int)SyncTime.FromSeconds(updatePeriod);
        }

        public void Run()
        {
            if (_disposed) throw new ObjectDisposedException($"{nameof(FXRuleUpdater)} on {_entityRef}");
            _cancellation = new CancellationTokenSource();
            AsyncUtils.RunAsyncTask(() => UpdateLoop(_cancellation.Token));
        }

        public void Dispose()
        {
            _disposed = true;
            _cancellation?.Cancel();
            _cancellation?.Dispose();
        }
        
        public void UpdateAnimators(Animator FXAnimator, Animator mainAnimator)
        {
            foreach (var fx in _fXs)
            {
                var animator = fx.Def.MainAnimatorProp ? mainAnimator : FXAnimator; 
                if (animator)
                    fx.Rule.UpdateValue(fx.Def, animator);
            }
        }
        
        private async Task UpdateLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var timeToContinue = SyncTime.NowUnsynced + _updatePeriod;

                    using (var wrapper = await NodeAccessor.Repository.Get(_entityRef.TypeId, _entityRef.Guid))
                    {
                        if (wrapper == null)
                        {
                            Logger.IfError()?.Message("wrapper is null " + _entityRef).Write();
                            return;
                        }

                        var ctx = new CalcerContext(wrapper, _entityRef, NodeAccessor.Repository);
                        foreach (var (rule, def) in _fXs)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await rule.CalcShouldDo(ctx, def);
                        }
                    }

                    var delay = (int)(timeToContinue - SyncTime.NowUnsynced);
                    if (delay > 0)
                        await Task.Delay(delay, cancellationToken);
                }
            } catch (OperationCanceledException) {}
        }
    }
}
