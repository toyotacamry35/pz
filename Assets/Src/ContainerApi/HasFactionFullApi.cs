using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public delegate void FactionChangedDelegate(FactionDef newFaction);

    public delegate void MutationStageChangedDelegate(MutationStageDef stage);

    public delegate void MutationChangedDelegate(float mutationRatio);

    public class HasFactionFullApi : EntityApi
    {
        private const string NewFactionPropertyName = nameof(IHasMutationMechanicsClientFull.MutationMechanics.NewFaction);
        private const string CurrentFactionPropertyName = nameof(IHasFactionClientFull.Faction);
        private const string MutationPropertyName = nameof(IHasMutationMechanicsClientFull.MutationMechanics.Mutation);
        private const string StagePropertyName = nameof(IHasMutationMechanicsClientFull.MutationMechanics.Stage);

        public event FactionChangedDelegate NewFactionChanged;
        public event FactionChangedDelegate CurrentFactionChanged;

        public event MutationStageChangedDelegate StageChanged;

        public event MutationChangedDelegate MutationChanged;

        private bool _isSubscribedOnProps;


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;

        public FactionDef NewFactionDef { get; private set; }

        public FactionDef CurrentFactionDef { get; private set; }

        public float Mutation { get; private set; }

        public MutationStageDef Stage { get; private set; }


        //=== Public ==========================================================

        public void SubscribeToNewFaction(FactionChangedDelegate onNewFactionChanged)
        {
            if (onNewFactionChanged.AssertIfNull(nameof(onNewFactionChanged)))
                return;

            NewFactionChanged += onNewFactionChanged;

            if (!_isSubscribedOnProps)
                return;

            onNewFactionChanged.Invoke(NewFactionDef);
        }

        public void SubscribeToCurrentFaction(FactionChangedDelegate onCurrentFactionChanged)
        {
            if (onCurrentFactionChanged.AssertIfNull(nameof(onCurrentFactionChanged)))
                return;

            CurrentFactionChanged += onCurrentFactionChanged;

            if (!_isSubscribedOnProps)
                return;

            onCurrentFactionChanged.Invoke(CurrentFactionDef);
        }

        public void SubscribeToMutation(MutationChangedDelegate onMutationChanged)
        {
            if (onMutationChanged.AssertIfNull(nameof(onMutationChanged)))
                return;

            MutationChanged += onMutationChanged;

            if (!_isSubscribedOnProps)
                return;

            onMutationChanged.Invoke(Mutation);
        }

        public void SubscribeToStage(MutationStageChangedDelegate onStageChanged)
        {
            if (onStageChanged.AssertIfNull(nameof(onStageChanged)))
                return;

            StageChanged += onStageChanged;

            if (!_isSubscribedOnProps)
                return;

            onStageChanged.Invoke(Stage);
        }

        public void UnsubscribeFromNewFaction(FactionChangedDelegate onNewFactionChanged)
        {
            if (onNewFactionChanged.AssertIfNull(nameof(onNewFactionChanged)))
                return;

            NewFactionChanged -= onNewFactionChanged;
        }

        public void UnsubscribeFromCurrentFaction(FactionChangedDelegate onCurrentFactionChanged)
        {
            if (onCurrentFactionChanged.AssertIfNull(nameof(onCurrentFactionChanged)))
                return;

            CurrentFactionChanged -= onCurrentFactionChanged;
        }

        public void UnsubscribeFromMutation(MutationChangedDelegate onMutationChanged)
        {
            if (onMutationChanged.AssertIfNull(nameof(onMutationChanged)))
                return;

            MutationChanged -= onMutationChanged;
        }

        public void UnsubscribeFromStage(MutationStageChangedDelegate onStageChanged)
        {
            if (onStageChanged.AssertIfNull(nameof(onStageChanged)))
                return;

            StageChanged -= onStageChanged;
        }


        //=== Protected =======================================================

        protected override Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var hasMutationMechanicsClientFull = wrapper as IHasMutationMechanicsClientFull;
            if (hasMutationMechanicsClientFull.AssertIfNull(nameof(hasMutationMechanicsClientFull)))
                return Task.CompletedTask;

            var deltaObject = hasMutationMechanicsClientFull;
            if (deltaObject.AssertIfNull(nameof(deltaObject)))
                return Task.CompletedTask;

            _isSubscribedOnProps = true;
            CurrentFactionDef = hasMutationMechanicsClientFull.Faction;
            NewFactionDef = hasMutationMechanicsClientFull.MutationMechanics.NewFaction;
            Mutation = hasMutationMechanicsClientFull.MutationMechanics.Mutation;
            Stage = hasMutationMechanicsClientFull.MutationMechanics.Stage;

            deltaObject.SubscribePropertyChanged(CurrentFactionPropertyName, OnCurrentFactionChanged);
            deltaObject.MutationMechanics.SubscribePropertyChanged(NewFactionPropertyName, OnNewFactionChanged);
            deltaObject.MutationMechanics.SubscribePropertyChanged(MutationPropertyName, OnMutationChanged);
            deltaObject.MutationMechanics.SubscribePropertyChanged(StagePropertyName, OnStageChanged);

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                NewFactionChanged?.Invoke(NewFactionDef);
                CurrentFactionChanged?.Invoke(CurrentFactionDef);
                StageChanged?.Invoke(Stage);
                MutationChanged?.Invoke(Mutation);
            });
            return Task.CompletedTask;
        }

        protected override Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            if (!_isSubscribedOnProps)
                return Task.CompletedTask;

            _isSubscribedOnProps = false;
            var hasMutationMechanicsClientFull = (IHasMutationMechanicsClientFull) wrapper;
            if (hasMutationMechanicsClientFull.AssertIfNull(nameof(hasMutationMechanicsClientFull)))
                return Task.CompletedTask;

            var deltaObject = hasMutationMechanicsClientFull;
            if (deltaObject.AssertIfNull(nameof(deltaObject)))
                return Task.CompletedTask;

            deltaObject.UnsubscribePropertyChanged(CurrentFactionPropertyName, OnCurrentFactionChanged);
            deltaObject.MutationMechanics.UnsubscribePropertyChanged(NewFactionPropertyName, OnNewFactionChanged);
            deltaObject.MutationMechanics.UnsubscribePropertyChanged(MutationPropertyName, OnMutationChanged);
            deltaObject.MutationMechanics.UnsubscribePropertyChanged(StagePropertyName, OnStageChanged);
            return Task.CompletedTask;
        }


        //=== Private =========================================================

        private async Task OnCurrentFactionChanged(EntityEventArgs args)
        {
            await OnSomeFactionChanged(args.NewValue as FactionDef, false);
        }

        private async Task OnNewFactionChanged(EntityEventArgs args)
        {
            await OnSomeFactionChanged(args.NewValue as FactionDef, true);
        }

        private Task OnSomeFactionChanged(FactionDef factionDef, bool isNewNorCurrent)
        {
            if (factionDef.AssertIfNull(nameof(factionDef)))
                return Task.CompletedTask;

            var someFactionDef = isNewNorCurrent ? NewFactionDef : CurrentFactionDef;
            var someEvent = isNewNorCurrent ? NewFactionChanged : CurrentFactionChanged;

            if (factionDef == someFactionDef)
                return Task.CompletedTask;

            someFactionDef = factionDef;
            if (isNewNorCurrent)
                NewFactionDef = factionDef;
            else
                CurrentFactionDef = factionDef;

            if (someEvent != null)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() => someEvent?.Invoke(someFactionDef));
            }

            return Task.CompletedTask;
        }

        private Task OnStageChanged(EntityEventArgs args)
        {
            var newStage = args.NewValue as MutationStageDef;
            if (Stage == newStage)
                return Task.CompletedTask;

            Stage = newStage;
            if (StageChanged != null)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() => StageChanged?.Invoke(Stage));
            }

            return Task.CompletedTask;
        }

        private Task OnMutationChanged(EntityEventArgs args)
        {
            var mutation = (float) args.NewValue;
            if (Mathf.Approximately(mutation, Mutation))
                return Task.CompletedTask;

            Mutation = mutation;
            if (MutationChanged != null)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() => MutationChanged?.Invoke(Mutation));
            }

            return Task.CompletedTask;
        }
    }
}