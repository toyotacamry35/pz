using Assets.Src.Aspects.Impl;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Character;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Assets.Src.Aspects
{
    class FactionTextureViewChanger : EntityGameObjectComponent
    {
        private new static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        protected override void GotClient()
        {
            //Subscribe(true);
        }

        protected override void LostClient()
        {
            //Subscribe(false);
        }

        private async Task OnStageChanged(MutationStageDef stage)
        {
            var newStage = stage;
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var mutationRefs = GetComponentsInChildren<MutationStageReference>(true);
                var currentMutationRef = mutationRefs.Where(v => v.Stages.Any(x => x.Target == newStage)).Single();
                foreach (var mutationRef in mutationRefs)
                    mutationRef.gameObject.SetActive(currentMutationRef == mutationRef);
            });
        }

        private async Task OnStageChanged(EntityEventArgs args)
        {
            var stage = (MutationStageDef)args.NewValue;
            await OnStageChanged(stage);
        }

        private void Subscribe(bool subscribe)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var characterC = await ClientRepo.Get<IWorldCharacter>(EntityId))
                {
                    var character = characterC.Get<IWorldCharacterClientBroadcast>(TypeId, EntityId, ReplicationLevel.ClientBroadcast);
                    if (subscribe)
                    {
                        character.MutationMechanics.SubscribePropertyChanged(nameof(IWorldCharacterClientBroadcast.MutationMechanics.Stage), OnStageChanged);
                        await OnStageChanged(character.MutationMechanics.Stage);
                    }
                    else
                        character.MutationMechanics.UnsubscribePropertyChanged(nameof(IWorldCharacterClientBroadcast.MutationMechanics.Stage), OnStageChanged);
                }
            });
        }
    }

    [Serializable]
    public class MutationStageView
    {
        public GenderDefRef[] Genders;
        public MutationStageDefRef[] Stages;
        public ThirdPersonCharacterView View;
    }
}
