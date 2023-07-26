using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.VisualMarkers;
using Assets.Src.InteractionSystem;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Wizardry;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Wizardry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Src.Aspects.Impl.Interaction
{
    public static class InteractionSpellBuffer
    {
        private static readonly int _concurrencyLevel = 5;
        private static ConcurrentQueue<MarkerInfoForSpell> _chooseSpellRequestsQueue = new ConcurrentQueue<MarkerInfoForSpell>();
        volatile private static Task _queueProcessingDispatcher;

        public static void Enqueue(this Interactive interactive,
            ISpellDoer spellDoerAsync, PredicateIgnoreGroupDef predicateIgnoreGroupDef,
            Action<InteractionSpellDefs> actionOnUpdate)
        {
            var markerInfo = new MarkerInfoForSpell(interactive, spellDoerAsync, predicateIgnoreGroupDef, actionOnUpdate);
            _chooseSpellRequestsQueue.Enqueue(markerInfo);

            var currentTask = _queueProcessingDispatcher;
            if (currentTask == null || currentTask.IsCompleted || _queueProcessingDispatcher.IsFaulted)
            {
                var newTask = new Task(() => { });
                var newCont = newTask.ContinueWith((t) => QueueProcessing()).Unwrap();
                var exchangedTask = Interlocked.CompareExchange(ref _queueProcessingDispatcher, newCont, currentTask);
                if (exchangedTask == currentTask)
                {
                    newTask.Start();
                    newTask.WrapErrors();
                }
            }
        }

        private static async Task QueueProcessing()
        {
            var runningTasks = new List<Task>();
            do
            {
                while (runningTasks.Count < _concurrencyLevel && _chooseSpellRequestsQueue.TryDequeue(out MarkerInfoForSpell job))
                    runningTasks.Add(JobProcessor(job));

                if (runningTasks.Count > 0)
                {
                    var completed = await Task.WhenAny(runningTasks);
                    runningTasks.Remove(completed);
                }
            } while (!_chooseSpellRequestsQueue.IsEmpty);
        }

        private static async Task JobProcessor(MarkerInfoForSpell job)
        {
            var interactionSpellDefs = new InteractionSpellDefs
            (
                interactionSpell: await job.SelectSpell(SpellDescription.InteractionAction),
                infoSpell: await job.SelectSpell(SpellDescription.InfoAction)
            );
            UnityQueueHelper.RunInUnityThreadNoWait(() => job.ActionOnUpdate(interactionSpellDefs));
        }
    }

    internal class MarkerInfoForSpell
    {
        internal Interactive Interactive;
        internal ISpellDoer SpellDoerAsync;
        internal PredicateIgnoreGroupDef PredicateIgnoreGroupDef;
        internal Action<InteractionSpellDefs> ActionOnUpdate;

        internal MarkerInfoForSpell(Interactive interactive,
            ISpellDoer spellDoerAsync, PredicateIgnoreGroupDef predicateIgnoreGroupDef,
            Action<InteractionSpellDefs> actionOnUpdate)
        {
            Interactive = interactive;
            SpellDoerAsync = spellDoerAsync;
            PredicateIgnoreGroupDef = predicateIgnoreGroupDef;
            ActionOnUpdate = actionOnUpdate;
        }

        public async Task<SpellDef> SelectSpell(ResourceRef<InputActionDef> interactionAction)
        {
            return await Interactive.ChooseSpell(SpellDoerAsync, interactionAction, PredicateIgnoreGroupDef, checkPredicatesOnly: true);
        }
    }

    public readonly struct InteractionSpellDefs
    {
        public readonly SpellDef InteractionSpell;
        public readonly SpellDef InfoSpell;

        public InteractionSpellDefs(SpellDef interactionSpell, SpellDef infoSpell)
        {
            InteractionSpell = interactionSpell;
            InfoSpell = infoSpell;
        }
    }
}