using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using Assets.Src.Wizardry;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Wizardry;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Modifiers;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects;
using UnityEngine;

namespace Assets.Src.InteractionSystem
{
    public class ContextualActions
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ContextualActions");

        private readonly IReadOnlyDictionary<InputActionDef, IReadOnlyList<IContextualAction>> _actions;

        // --- C-tors: ----------------------------------------------------------------

        public ContextualActions(ContextualActionsDef def)
        {
            if (def == null)
            {
                Debug.LogError($"{nameof(ContextualActions)}() <{nameof(ContextualActionsDef)}> def is null");
                _actions = new Dictionary<InputActionDef, IReadOnlyList<IContextualAction>>();
                return;
            }

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Got contextual actions {def.SpellsByAction.Count}").Write();

            _actions = def.SpellsByAction
                .Select(actionRef => ValueTuple.Create(actionRef.Key.Target, GetListOfContextualActionFromDef(actionRef.Value)))
                .ToDictionary(v => v.Item1, v => v.Item2);
        }


        // --- API: ----------------------------------------------------------------

        internal async Task<List<KeyValuePair<InputActionDef, IContextualAction>>> GetSpellsFor([NotNull] ISpellDoer caster, OuterRef<IEntityObject> target) //t-p
        {
            var actions = new List<KeyValuePair<InputActionDef, IContextualAction>>();
            foreach (var action in _actions)
            {
                var contextualAction = await GetSpellFor(caster, target, action.Key); //t-p
                if (contextualAction != null)
                    actions.Add(new KeyValuePair<InputActionDef, IContextualAction>(action.Key, contextualAction));
            }

            return actions;
        }

        internal async Task<IContextualAction> GetSpellFor(
            [NotNull] ISpellDoer caster, 
            OuterRef<IEntityObject> target, 
            InputActionDef inputAction,
            bool checkActualSpellToo = false, 
			PredicateIgnoreGroupDef predicateIgnoreGroupDef = null, 
			bool checkPredicatesOnly = false) //t-p
        {
            IReadOnlyList<IContextualAction> list;
            if (!_actions.TryGetValue(inputAction, out list))
                return null;
            return await GetFirstAction(caster, target, list, checkActualSpellToo, predicateIgnoreGroupDef, checkPredicatesOnly); //t-p
        }

        private static async Task<IContextualAction> GetFirstAction(
            [NotNull] ISpellDoer caster,
            OuterRef<IEntityObject> target,
            [NotNull] IEnumerable<IContextualAction> actions, 
            bool checkActualSpellToo = false,
			PredicateIgnoreGroupDef predicateIgnoreGroupDef = null, 
			bool checkPredicatesOnly = false) //t-p
        {
            if (caster == null) throw new ArgumentNullException(nameof(caster));
            if (actions == null) throw new ArgumentNullException(nameof(actions));
            
            var castBuilder = new SpellCastBuilder().SetTargetIfValid(target);
            foreach (var contextualAction in actions)
            {
                //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Tested contextualAction with spellDef '{contextualAction.SpellDef}'...").Write();
                if (contextualAction.CheckSpellDef != null)
                {
                    if (!await caster.CanStartCast(castBuilder.SetSpell(contextualAction.CheckSpellDef).Build(), predicateIgnoreGroupDef, checkPredicatesOnly))
                    {
                        //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Don't pass by CheckSpellDef='{contextualAction.CheckSpellDef}'").Write();
                        continue;
                    }
                }

                if (checkActualSpellToo || contextualAction.CheckSpellDef == null)
                {
                    if (!await caster.CanStartCast(castBuilder.SetSpell(contextualAction.SpellDef).Build(), predicateIgnoreGroupDef, checkPredicatesOnly))
                    {
                        //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Don't pass by SpellDef='{contextualAction.CheckSpellDef}'").Write();
                        continue;
                    }
                }

                //if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Selected").Write();;
                return contextualAction;
            }

            return null;
        }

        internal static async Task<List<IContextualAction>> GetFirstActions(
            [NotNull] ISpellDoer caster,
            [CanBeNull] EntityGameObject target, 
            Vector3 direction, 
            [NotNull] List<IContextualAction> actions,
            bool checkActualSpellToo = false) //t-p
        {
            var suitableActions = new List<IContextualAction>();
            var castBuilder = new SpellCastBuilder().SetDirection(direction.ToShared());
            if (target)
                castBuilder = castBuilder.SetTarget(target.OuterRef);
            foreach (var contextualAction in actions)
            {
                if (suitableActions.Count > 0 && contextualAction.Priority > suitableActions[0].Priority)
                    break;

                if (contextualAction.CheckSpellDef != null)
                {
                    if (!await caster.CanStartCast(castBuilder.SetSpell(contextualAction.CheckSpellDef).Build()))
                        continue;
                }

                if (checkActualSpellToo || contextualAction.CheckSpellDef == null)
                {
                    if (!await caster.CanStartCast(castBuilder.SetSpell(contextualAction.SpellDef).Build()))
                        continue;
                }

                suitableActions.Add(contextualAction);
            }

            return suitableActions;
        }

        internal static async Task<List<IContextualAction>> GetFirstActions(
            [NotNull] IWizardEntityServer casterWizard,
            OuterRef<IEntity> targetRef,
            SharedCode.Utils.Vector3 direction,
            [NotNull] List<IContextualAction> actions,
            bool checkActualSpellToo = false)
        {
            var castBuilder = new SpellCastBuilder()
                .SetTargetIfValid(targetRef)
                .SetDirection(direction);

            var result = new List<IContextualAction>();
            foreach (var ca in actions)
            {
                if (result.Count > 0 && ca.Priority > result[0].Priority)
                    break;

                bool canSelect;
                bool checkActualSpell = checkActualSpellToo;
                if (ca.CheckSpellDef != null)
                {
                    castBuilder.SetSpell(ca.CheckSpellDef);
                    var spellCast = castBuilder.Build();
                    canSelect = await casterWizard.CheckSpellCastPredicates(SyncTime.Now, spellCast, null, null);
                }
                else
                {
                    checkActualSpell = true;
                    canSelect = true;
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Contextual action {ca.SpellDef} check spell {canSelect}").Write();

                if (checkActualSpell && canSelect)
                {
                    castBuilder.SetSpell(ca.SpellDef);
                    var spellCast = castBuilder.Build();
                    canSelect = await casterWizard.CheckSpellCastPredicates(SyncTime.Now, castBuilder.Build(), null, null);
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Contextual action {ca.SpellDef} can select {canSelect}").Write();

                if (canSelect)
                    result.Add(ca);
            }

            return result;
        }

        internal static IReadOnlyList<IContextualAction> GetListOfContextualActionFromDef(
            Dictionary<int, ResourceRef<ContextualActionDef>> actionsDefDic)
        {
            return actionsDefDic
                .Select(v => new ContextualActionInstance2(v.Key, v.Value.Target.Spell.Target, v.Value.Target.CheckSpell.Target))
                .OrderBy(v => v.Priority).ToArray();
        }

        internal static Comparison<IContextualAction> SortByPriority = (x, y) => x.Priority.CompareTo(y.Priority);
    }

    // --- Internal types: ----------------------------------------------------------------
}