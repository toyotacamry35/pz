using System;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using Assets.Src.GameObjectAssembler;
using Assets.Src.InteractionSystem;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using Assets.Src.Wizardry;
using Assets.Tools;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Wizardry;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using L10n;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using UnityEngine;
using UnityEngine.Serialization;
using SharedCode.Serializers;

namespace Assets.Src.Aspects.Impl
{
    public class Interactive : MonoBehaviour, IFromDef<InteractiveDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const float SpellDescriptionUpdatePeriod = 1;
        
        public InteractionType InteractionType;
        public InteractionType KnowType = InteractionType.CollectMiddle;

        [FormerlySerializedAs("_localObjectDefRef")]
        public NonEntityObjectDefRef LocalObjectDefRef;

        public int Priority = 1;

        private SpellDescriptions _spellDescriptions;

        [CanBeNull]
        private ISpellDoer _spellDoer;

        [CanBeNull]
        private EntityGameObject _ego;

        private static SpellDescriptions EmptyDescriptions;

        private bool _isWaitingForDescriptions;
        private float _spellDescriptionsLastUpdateTime;
        private IEntitiesRepository Repository => _ego ? _ego.ClientRepo : null;
        private OuterRef<IEntityObject> OuterRef => _ego ? _ego.OuterRef : OuterRef<IEntityObject>.Invalid;


        //=== Props ===========================================================

        public LocalizedString LocalNameLs => LocalObjectDefRef?.Target?.NameLs ?? LsExtensions.Empty;

        private SpellDef LocalInteractionSpellDef => LocalObjectDefRef?.Target?.SpellDef.Target;

        public NonEntityObjectDef LocalObjectDef => LocalObjectDefRef.Target; //Для отладки public

        public IEntityObjectDef EntityObjectDef => _ego?.EntityDef;

        public SpellDef InteractionSpellDef { get; private set; }

        public SpellDef AttackSpellDef { get; private set; }

        private InteractiveDef _def;

        public InteractiveDef Def
        {
            get => _def;
            set
            {
                _def = value;
                if (_def != null)
                {
                    if (_def.Interaction != InteractionType.None)
                    {
                        InteractionType = _def.Interaction;
                    }

                    if (_def.Know != InteractionType.None)
                    {
                        KnowType = _def.Know;
                    }

                    if (LocalInteractionSpellDef != null)
                        InteractionSpellDef = LocalInteractionSpellDef;

                    if (_def.BasicInteractionSpell.Target != null)
                        InteractionSpellDef = _def.BasicInteractionSpell.Target; //значение LocalInteractionSpellDef подавляется

                    if (_def.BasicAttackSpell.Target != null)
                        AttackSpellDef = _def.BasicAttackSpell.Target;
                }
            }
        }


        private ContextualActions Actions
        {
            get
            {
                // This is temporary fix to prevent Unity crash.
                // This class should be rewritten to avoid additional load induced by this fix.
                return _def?.ContextualActions.Target != null ? new ContextualActions(_def.ContextualActions.Target) : null;
            }
        }

        //=== Public ==========================================================

        private async Task UpdateSpellDescriptions(ISpellDoer forSubject) //tp-c
        {
            if (forSubject.AssertIfNull(nameof(forSubject)))
                return;

            var newSpellDescriptions = new SpellDescriptions();
            var actions = Actions;
            if (actions != null)
            {
                var contextualActionsKvp = await actions.GetSpellsFor(forSubject, OuterRef);
                foreach (var kvp in contextualActionsKvp)
                    await UpdateInteractionDescriptionsByActionType(newSpellDescriptions, kvp.Value.SpellDef, kvp.Key);
            }

            await UpdateLegacySpellDescriptions(forSubject, newSpellDescriptions, SpellDescription.InteractionAction, InteractionSpellDef, true);

            lock (_spellDescriptions)
            {
                _spellDescriptions = newSpellDescriptions;
            }

            _isWaitingForDescriptions = false;
        }

        private async Task UpdateLegacySpellDescriptions(ISpellDoer forSubject, SpellDescriptions spellDescriptions, InputActionDef inputAction, 
            SpellDef spell, bool lookForLocalDescription = false) //t-p
        {
            if (spell != null && !spellDescriptions.Descriptions.ContainsKey(inputAction))
            {
                //new PredicateIgnoreGroup[] { PredicateIgnoreGroup.InternalRestrictions }
                var spellCastOrder = new SpellCastBuilder().SetSpell(spell).SetTargetIfValid(OuterRef).Build();
                var canStartCast = await forSubject.CanStartCast(spellCastOrder);
                if (canStartCast)
                    await UpdateInteractionDescriptionsByActionType(spellDescriptions, spell, inputAction, lookForLocalDescription);
            }
        }

        public SpellDescriptions GetSpellDescriptions(GameObject forSubject) //u-c
        {
            if (!_isWaitingForDescriptions && Time.time - _spellDescriptionsLastUpdateTime > SpellDescriptionUpdatePeriod)
            {
                _spellDescriptionsLastUpdateTime = Time.time;
                _isWaitingForDescriptions = true;
                var caster = forSubject.GetComponent<ISubjectWithSpellDoer>()?.SpellDoer;
                if (caster != null)
                    AsyncUtils.RunAsyncTask(() => UpdateSpellDescriptions(caster));
            }

            if (_spellDescriptions == null)
                return EmptyDescriptions;

            lock (_spellDescriptions)
            {
                return _spellDescriptions;
            }
        }

        public async Task<SpellDef> ChooseSpell(ISpellDoer with, InputActionDef action,
            PredicateIgnoreGroupDef predicateIgnoreGroupDef = null, bool checkPredicatesOnly = false) //t-p
        {
            if (with == null)
            {
                if (Logger.IsDebugEnabled) Logger.IfError()?.Message($"SpellDoer is null | Action:{action} PredicateIgnoreGroup:{predicateIgnoreGroupDef}").Write();
                return null;
            }
            
            SpellDef spell = null;
            var actions = Actions;
            if (actions != null)
                spell = (await actions.GetSpellFor(with, OuterRef, action, false, predicateIgnoreGroupDef, checkPredicatesOnly))?.SpellDef;
            //legacy
            if (spell == null)
                if (AttackSpellDef != null && action == SpellDescription.AttackAction)
                {
                if (await with.CanStartCast(
                    new SpellCastBuilder().SetSpell(AttackSpellDef).SetTargetIfValid(OuterRef).Build(), predicateIgnoreGroupDef, checkPredicatesOnly))
                        spell = AttackSpellDef;
                }
            if (spell == null)
                if (InteractionSpellDef != null && action == SpellDescription.InteractionAction)
                {
                if (await with.CanStartCast(
                    new SpellCastBuilder().SetSpell(InteractionSpellDef).SetTargetIfValid(OuterRef).Build(), predicateIgnoreGroupDef, checkPredicatesOnly))
                        spell = InteractionSpellDef;
                }
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Action:{action.ActionToString()} Spell:{spell} IgnoreGroup:{predicateIgnoreGroupDef}").Write();
            return spell;
        }

        // to avoid "get_gameObject can only be called from the main thread." at asyncs
        [HideInInspector]
        public GameObject GameObject;

        private void Awake()
        {
            GameObject = gameObject;
            if (EmptyDescriptions == null)
                EmptyDescriptions = new SpellDescriptions();
            _spellDescriptions = EmptyDescriptions;

            if (LocalInteractionSpellDef != null && InteractionSpellDef == null)
                InteractionSpellDef = LocalInteractionSpellDef;

            var root = gameObject.GetRoot();
            _ego = root.GetComponent<EntityGameObject>();
        }

        public async Task<SpellDef> ChooseSpellDefForSelf(OuterRef<IEntityObject> with, InputActionDef interactionType)
        {
            IContextualAction contextualAction = null;
            var actions = Actions;
            if (actions != null)
            {
                if( _spellDoer == null )
                    using (var cnt = await Repository.Get(OuterRef))
                    {
                        var hasWizard = cnt.Get<IHasWizardEntityClientFull>(OuterRef);
                        _spellDoer = hasWizard?.SlaveWizardHolder.SpellDoer;
                    }
                if (_spellDoer == null ) throw new Exception($"No Spell Doer for {nameof(Interactive)} {OuterRef}");
                contextualAction = await actions.GetSpellFor(_spellDoer, with, interactionType);
            }

            return contextualAction?.SpellDef;
        }

        public async Task OnInteraction(ISpellDoer with, InputActionDef interactionKey) //unity context
        {
            with.DoCast(new SpellCastBuilder().SetTargetIfValid(OuterRef).SetSpell(await ChooseSpell(with, interactionKey)).Build());
        }


        //=== Private =========================================================

        /// <summary>
        /// Собирает описания доступных в настоящий момент спеллов
        /// </summary>
        private async Task UpdateInteractionDescriptionsByActionType(SpellDescriptions spellDescriptions, SpellDef spell, InputActionDef inputAction,
            bool lookForLocalDescription = false)
        {
            var newDescription = await UnityQueueHelper.RunInUnityThread(() =>
                SpellDescription.GetSpellDescription(spell, GameObject, lookForLocalDescription));

            if (spellDescriptions.Descriptions.TryGetValue(inputAction, out var existingDescription))
                existingDescription.Merge(newDescription);
            else
                spellDescriptions.Descriptions[inputAction] = newDescription;
        }
    }
}