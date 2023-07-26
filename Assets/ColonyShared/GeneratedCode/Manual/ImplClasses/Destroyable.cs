using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using GeneratedCode.DeltaObjects.Chain;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public partial class Destroyable : IHookOnInit
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task<bool> DestroyImpl()
        {
            Logger.IfDebug()?.Message($"Destroyable: DestroyImpl. parentEntity.TypeId = {parentEntity.TypeId}, parentEntity.Id = {parentEntity.Id}").Write();

            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("IDestroyable.Destroy").Write();;
            var repo = EntitiesRepository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                await repo.Destroy(parentEntity.TypeId, parentEntity.Id);
            }, EntitiesRepository);
            return Task.FromResult(true);
        }

        public Task<ChainCancellationToken> DestroyAfterDelayImpl(float delay)
        {
            switch(parentEntity)
            {
                case IObelisk entity:
                    return Task.FromResult(entity.Chain().Delay(delay).Destroy().Run());
                case IInteractiveEntity entity:
                    return Task.FromResult(entity.Chain().Delay(delay).Destroy().Run());
                case SharedCode.Entities.Mineable.IMineableEntity entity:
                    return Task.FromResult(entity.Chain().Delay(delay).Destroy().Run());
                case SharedCode.AI.ILegionaryEntity entity:
                    return Task.FromResult(entity.Chain().Delay(delay).Destroy().Run());
                case SharedCode.Entities.IWorldCorpse entity:
                    return Task.FromResult(entity.Chain().Delay(delay).Destroy().Run());
                case SharedCode.Entities.IWorldBox entity:
                    return Task.FromResult(entity.Chain().Delay(delay).Destroy().Run());
                default:
                    Logger.IfError()?.Message("Unknown Implementation: {0}", parentEntity.TypeName).Write();
                    return null;
            }
        }


        // --- IHookOnInit: -------------------------------------------

        public Task OnInit()
        {
            // Subscribe `Destroy` on `IHasLifespan` if is
            var hasLifespan = parentEntity as IHasLifespan;
            if (hasLifespan != null)
                hasLifespan.Lifespan.LifespanExpiredEvent += async (Guid entityId, int typeId, OnLifespanExpired whatToDo) =>
                {
                    if (whatToDo != OnLifespanExpired.Destroy)
                        return;

                    await Destroy();
                };
            return Task.CompletedTask;
        }
    }
}