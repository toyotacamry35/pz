using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class FactionIndicator : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private FactionDefRef _humanFactionDefRef;

        private MutatingFactionDef _humanFactionDef;
        private float _maxMutation;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_humanFactionDefRef.AssertIfNull(nameof(_humanFactionDefRef)))
                return;

            _humanFactionDef = _humanFactionDefRef.Target;
            _humanFactionDef.AssertIfNull(nameof(_humanFactionDef));
            _maxMutation = _humanFactionDef?.MaxMutation ?? 1;
        }


        //=== Props ===========================================================

        [Binding, UsedImplicitly]
        public float MutationRatio { get; private set; }

        [Binding, UsedImplicitly]
        public bool IsHumanFaction { get; private set; }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource)
        {
            var mutationStream = pawnSource.TouchableEntityProxy
                .Child(D, character => character.MutationMechanics)
                .ToStream(D, factionClientFull => factionClientFull.Mutation);

            var newFactionStream = pawnSource.TouchableEntityProxy
                .Child(D, character => character.MutationMechanics)
                .ToStream(D, factionClientFull => factionClientFull.NewFaction);

            var mutationRatioStream = mutationStream.Func(D, mutation => Mathf.Clamp01(mutation / _maxMutation));
            Bind(mutationRatioStream, () => MutationRatio);

            var isHumanNewFactionStream = newFactionStream.Func(D, newFactionDef => newFactionDef == _humanFactionDef);
            Bind(isHumanNewFactionStream, () => IsHumanFaction);
        }
    }
}