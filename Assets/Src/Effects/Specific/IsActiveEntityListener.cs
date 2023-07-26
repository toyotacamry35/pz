using Assets.Src.Aspects;
using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System;
using System.Threading.Tasks;
using Assets.Tools;
using Core.Environment.Logging.Extension;

public abstract class IsActiveEntityListener : EntityGameObjectComponent
{
    protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    protected override void GotServer()
    {
     //   Subscribe();
    }

    protected override void GotClient()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        var repository = ClientRepo;
        var entityRef = GetOuterRef<IEntity>();
        AsyncUtils.RunAsyncTask(async () =>
        {
            using (var wrapper = await repository.Get(entityRef))
            {
                var entity = wrapper.Get<ICanBeActiveClientBroadcast>(entityRef, ReplicationLevel.ClientBroadcast);
                if (entity == null)
                {
                    Logger.IfError()?.Message($"entity is null entityRef:{entityRef}").Write();
                    return;
                }
                ((IEntity)entity).SubscribePropertyChanged(nameof(entity.IsActive), IsActiveChanged);
                await SetActive(entity.IsActive, IsClient && !IsServer);
            }
        });
    }

    private async Task IsActiveChanged(EntityEventArgs args)
    {
        // Logger.IfInfo()?.Message("IsActiveChanged").Write();;
        await SetActive((bool)args.NewValue, IsClient && !IsServer);
    }

    protected abstract Task SetActive(bool activate, bool isClient);
}
